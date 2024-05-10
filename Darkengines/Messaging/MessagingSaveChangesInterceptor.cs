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

namespace Darkengines.Data {
	public class MessagingSaveChangesInterceptor : SaveChangesInterceptor {
		protected IEnumerable<IRuleMap> RuleMaps { get; }
		protected IApplicationContext ApplicationContext { get; }
		protected MessagingSystem MessagingSystem { get; }
		public MessagingSaveChangesInterceptor(
			IApplicationContext applicationContext,
			MessagingSystem messagingSystem,
			IEnumerable<IRuleMap> ruleMaps
		) {
			ApplicationContext = applicationContext;
			RuleMaps = ruleMaps;
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
				entryRuleMaps.Select(entryRuleMaps => {
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
						predicateExpression,
						contextParameterExpression,
						instanceExpression
					);
					var predicate = lambdaExpression.Compile();
					foreach (var entry in entryRuleMaps.EntryGroup.Value) {
						var primaryKey = entry.Metadata.FindPrimaryKey();
						var properties = entry.Properties.Where(property => property.IsModified).ToArray();
						properties.Join()
							foreach (var user in userClients) {
							var result = predicate(new MessagingApplicationContext(user.Key), entry.Entity);
							if (result) {
							}
						}
					}
				})
				foreach (var entry in eventData.Context.ChangeTracker.Entries()) {
					if (ruleMaps.TryGetValue(entry.Metadata.ClrType, out var ruleMap)) {
						ruleMap.GetOperationResolver(Operation.Read,)
						}
				}
				return await base.SavingChangesAsync(eventData, result, cancellationToken);
			}
		}
	}
}
