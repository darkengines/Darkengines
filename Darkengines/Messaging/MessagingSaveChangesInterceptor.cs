using Darkengines.Data;
using Darkengines.Applications;
using Darkengines.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Darkengines.Expressions.Security;
using Darkengines.WebSockets;
using Darkengines.Expressions;
using System.Linq.Expressions;
using Darkengines.Messaging;
using Darkengines.Expressions.Converters.Javascript;
using System.Reflection;
using static Darkengines.Data.MessagingSaveChangesInterceptor;
using Newtonsoft.Json;
using System.Text;

namespace Darkengines.Data {
	public class MessagingSaveChangesInterceptor : SaveChangesInterceptor {
		protected IEnumerable<IRuleMap> RuleMaps { get; }
		protected IApplicationContext ApplicationContext { get; }
		protected MessagingSystem MessagingSystem { get; }
		protected JsonSerializer JsonSerializer { get; }
		public MessagingSaveChangesInterceptor(
			IApplicationContext applicationContext,
			MessagingSystem messagingSystem,
			IEnumerable<IRuleMap> ruleMaps,
			JsonSerializer jsonSerializer
		) {
			ApplicationContext = applicationContext;
			RuleMaps = ruleMaps;
			JsonSerializer = jsonSerializer;
			MessagingSystem = messagingSystem;
		}

		public class ResolverRegistry {
			public required Expression InstanceExpression { get; set; }
			public IDictionary<PropertyInfo, Expression> InstancePropertyExpression { get; }
			public ResolverRegistry() {
				InstancePropertyExpression = new Dictionary<PropertyInfo, Expression>();
			}
		}

		public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			if (eventData.Context is ApplicationDbContext applicationDbContext) {
				var clients = MessagingSystem.GetClients();
				var userClients = clients.GroupBy(client => client.Identity).ToDictionary(userClients => userClients.Key);
				var ruleMaps = RuleMaps.GroupBy(ruleMap => ruleMap.Type).ToDictionary(ruleMaps => ruleMaps.Key);
				var entries = eventData.Context.ChangeTracker.Entries().GroupBy(entry => entry.Metadata.ClrType).ToDictionary(entries => entries.Key);
				var entryRuleMaps = ruleMaps.Join(
					entries,
					ruleMap => ruleMap.Key,
					entryGroup => entryGroup.Key,
					(ruleMapGroup, entryGroup) => new {
						RuleMapGroup = ruleMapGroup,
						EntryGroup = entryGroup
					}
				);
				var predicates = entryRuleMaps.Select(entryRuleMaps => {
					var instanceExpression = Expression.Parameter(entryRuleMaps.EntryGroup.Key);
					var contextParameterExpression = Expression.Parameter(typeof(IApplicationContext));
					var propertyInfos = entryRuleMaps.RuleMapGroup.Key.GetProperties().ToArray();
					var resolverRegistry = new ResolverRegistry {
						InstanceExpression = Expression.Constant(false)
					};
					foreach (var propertyInfo in propertyInfos) {
						resolverRegistry.InstancePropertyExpression[propertyInfo] = Expression.Constant(false);
					}
					resolverRegistry = entryRuleMaps.RuleMapGroup.Value.Aggregate(
						resolverRegistry,
						(resolverRegistry, rulemap) => {
							resolverRegistry = propertyInfos.Select(propertyInfo =>
								new {
									Resolver = rulemap.GetPropertyOperationResolver(
										propertyInfo,
										Operation.Read,
										contextParameterExpression,
										Expression.Convert(instanceExpression, entryRuleMaps.EntryGroup.Key)
									),
									PropertyInfo = propertyInfo
								}
							).Where(tuple => tuple.Resolver != null)
							.Aggregate(
								resolverRegistry,
								(resolverRegistry, tuple) => {
									resolverRegistry.InstancePropertyExpression[tuple.PropertyInfo] = Expression.OrElse(
										resolverRegistry.InstancePropertyExpression[tuple.PropertyInfo],
										tuple.Resolver
									);
									return resolverRegistry;
								}
							);

							resolverRegistry.InstanceExpression = Expression.OrElse(
								resolverRegistry.InstanceExpression,
								rulemap.GetOperationResolver(
									Operation.Read,
									contextParameterExpression,
									Expression.Convert(instanceExpression, entryRuleMaps.EntryGroup.Key)
								)
							);
							return resolverRegistry;
						}
					);
					var lambdaExpression = Expression.Lambda<Func<IApplicationContext, object, bool>>(
						resolverRegistry.InstanceExpression,
						contextParameterExpression,
						instanceExpression
					);
					var propertyLambdaExpressions = resolverRegistry.InstancePropertyExpression.Select(pair => {
						var lambdaExpression = Expression.Lambda<Func<IApplicationContext, object, bool>>(
							pair.Value,
							contextParameterExpression,
							instanceExpression
						);
						return new {
							PropertyInfo = pair.Key,
							Expression = pair.Value,
							LambdaExpression = lambdaExpression,
							Predicate = lambdaExpression.Compile()
						};
					}).ToDictionary(tuple => tuple.PropertyInfo);
					var predicate = lambdaExpression.Compile();
					return new {
						Type = entryRuleMaps.EntryGroup.Key,
						InstancePredicate = predicate,
						PropertyPredicates = propertyLambdaExpressions
					};
				}).ToDictionary(tuple => tuple.Type);


				foreach (var entryRuleMap in entryRuleMaps) {
					if (!predicates.TryGetValue(entryRuleMap.RuleMapGroup.Key, out var predicate)) continue;
					foreach (var user in userClients) {
						var context = new MessagingApplicationContext(user.Key);
						foreach (var entry in entryRuleMap.EntryGroup.Value) {
							if (!predicate.InstancePredicate.Invoke(context, entry.Entity)) continue;
							var primaryKey = entry.Metadata.FindPrimaryKey();
							var propertyPredicates = entry.Properties.Where(property => property.IsModified)
							.Join(
								predicate.PropertyPredicates,
								property => property.Metadata.PropertyInfo,
								predicate => predicate.Key,
								(property, predicate) => (property, predicate)
							).ToArray();
							if (!propertyPredicates.Any()) continue;
							var delta = Activator.CreateInstance(entry.Metadata.ClrType);
							foreach (var primaryKeyProperty in primaryKey.Properties) {
								primaryKeyProperty.PropertyInfo.SetValue(delta, primaryKeyProperty.PropertyInfo.GetValue(entry.Entity));
							}
							foreach (var propertyPredicate in propertyPredicates) {
								if (!propertyPredicate.predicate.Value.Predicate(context, entry.Entity)) continue;
								propertyPredicate.property.Metadata.PropertyInfo.SetValue(delta, propertyPredicate.property.Metadata.PropertyInfo.GetValue(entry.Entity));
							}
							foreach (var client in user.Value) {
								var writer = new StringWriter();
								JsonSerializer.Serialize(writer, delta);
								var serialized = writer.ToString();
								await client.SendMessageAsync(client.WebSocket, Encoding.UTF8.GetBytes(serialized));
							}
						}
					}
				}
			}
			return await base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}