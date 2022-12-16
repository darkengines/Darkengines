using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Mutation {
	public class EntityMutationInfo {
		public static MethodInfo WhereMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IQueryable<object>, Func<Expression<Func<object, bool>>, IQueryable<object>>>(context => context.Where).GetGenericMethodDefinition();
		public static MethodInfo SelectMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IQueryable<object>, Func<Expression<Func<object, object>>, IQueryable<object>>>(queryable => queryable.Select).GetGenericMethodDefinition();
		public static MethodInfo FirstOrDefaultMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IQueryable<object>, Func<object>>(queryable => queryable.FirstOrDefault).GetGenericMethodDefinition();
		public static MethodInfo CollectionWhereMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IEnumerable<object>, Func<Func<object, bool>, IEnumerable<object>>>(context => context.Where).GetGenericMethodDefinition();
		public static MethodInfo CollectionSelectMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IEnumerable<object>, Func<Func<object, object>, IEnumerable<object>>>(queryable => queryable.Select).GetGenericMethodDefinition();
		public static MethodInfo CollectionFirstOrDefaultMethodInfo { get; } = ExpressionHelper.ExtractMethodInfo<IEnumerable<object>, Func<object>>(queryable => queryable.FirstOrDefault).GetGenericMethodDefinition();

		protected IEntityType EntityType { get; }
		protected JObject JObject { get; }
		public JObject FilteredJObject { get; }
		public MutationContext EntityMutationContext { get; }
		public object Entity { get; }
		public Type PermissionEntityType { get; }
		public PropertyInfo SelfPermissionProperty { get; }
		public IKey? PrimaryKey { get; }
		public IRuleMap RuleMap { get; }
		protected IDictionary<IPropertyBase, PropertyMutationInfo> PropertyMutationInfos { get; }
		protected IDictionary<IPropertyBase, ReferenceMutationInfo> ReferenceMutationInfos { get; }
		protected IDictionary<IPropertyBase, CollectionMutationInfo> CollectionMutationInfos { get; }
		protected EntityState EntityState { get; }
		protected bool IsReference { get; }
		public EntityMutationInfo(IEntityType entityType, JObject jObject, MutationContext mutationContext) {
			IsReference = jObject.ContainsKey("$ref");
			EntityType = entityType;
			JObject = jObject;
			if (JObject.ContainsKey("$id")) {
				var referenceId = (string)JObject["$id"]!;
				mutationContext.JObjectReferenceMapping[referenceId] = jObject;
			}
			Entity = jObject.ToObject(entityType.ClrType);
			mutationContext.JObjectEntityMutationInfoMapping[JObject] = this;
			EntityMutationContext = mutationContext;
			PermissionEntityType = EntityMutationContext.PermissionEntityTypeBuilder.TypeMap[EntityType];
			SelfPermissionProperty = PermissionEntityType.GetProperty("SelfPermission");
			PrimaryKey = EntityType.FindPrimaryKey();

			var ruleMap = EntityMutationContext.RuleMaps.FirstOrDefault(ruleMap => ruleMap.Type == entityType.ClrType);
			if (ruleMap == null) throw new InvalidOperationException($"No rulemap found for type {entityType.ClrType.Name}");
			RuleMap = ruleMap;

			var jProperties = JObject.Properties();
			var properties = EntityType.GetProperties();
			var navigations = EntityType.GetNavigations();
			var references = navigations.Where(navigation => !navigation.IsCollection);
			var collections = navigations.Where(navigation => navigation.IsCollection);

			PropertyMutationInfos = jProperties.Join(properties, jp => jp.Name.ToPascalCase(), p => p.Name, (jp, p) => new PropertyMutationInfo(this, p, jp)).ToDictionary(pmi => (IPropertyBase)pmi.Property);
			ReferenceMutationInfos = jProperties.Join(references, jp => jp.Name.ToPascalCase(), p => p.Name, (jp, p) => new ReferenceMutationInfo(this, p, jp)).ToDictionary(pmi => pmi.Property);
			CollectionMutationInfos = jProperties
				.Join(collections, jp => jp.Name.ToPascalCase(), p => p.Name, (jp, p) => (JProperty: jp, Property: p))
				.Where(tuple => tuple.JProperty.Values().Any())
				.Select(tuple => new CollectionMutationInfo(this, tuple.Property, tuple.JProperty)).ToDictionary(pmi => pmi.Property);

			var filteredJProperties = PropertyMutationInfos.Values.Select(propertyMutationInfo => propertyMutationInfo.JProperty)
			.Concat(ReferenceMutationInfos.Values.Select(referenceMutationInfo => new JProperty(referenceMutationInfo.JProperty.Name, referenceMutationInfo.TargetEntityMutationInfo.FilteredJObject)))
			.Concat(CollectionMutationInfos.Values.Select(collectionMutationInfo => new JProperty(
				collectionMutationInfo.JProperty.Name,
				new JArray(collectionMutationInfo.TargetEntityMutationInfos.Select(targetEntityMutationInfo => targetEntityMutationInfo.FilteredJObject)))
			));

			FilteredJObject = new JObject(filteredJProperties);
		}

		public EntityEntry GetEntry() {
			var entity = Activator.CreateInstance(EntityType.ClrType)!;

			var primaryKey = EntityType.FindPrimaryKey()!;
			var primaryKeyMutationInfos = new Dictionary<IPropertyBase, PropertyMutationInfo>(PropertyMutationInfos.Where(keyValuePair => keyValuePair.Value.Property.IsPrimaryKey()));
			var isPrimaryKeySet = primaryKeyMutationInfos.Count == primaryKey.Properties.Count;
			var nonGeneratedPropertyTuples = new Dictionary<IPropertyBase, PropertyMutationInfo>(primaryKeyMutationInfos.Where(pmi => !pmi.Value.Property.ValueGenerated.HasFlag(ValueGenerated.OnAdd)));

			foreach (var primaryKeyMutationInfo in primaryKeyMutationInfos) primaryKeyMutationInfo.Key.PropertyInfo!.SetValue(entity, primaryKeyMutationInfo.Value.JProperty.Value.ToObject(primaryKeyMutationInfo.Key.ClrType));
			var entry = EntityMutationContext.DbContext.Attach(entity);
			foreach (var propertyMutationInfo in PropertyMutationInfos) propertyMutationInfo.Key.PropertyInfo!.SetValue(entity, propertyMutationInfo.Value.JProperty.Value.ToObject(propertyMutationInfo.Key.ClrType));
			foreach (var referenceMutationInfo in ReferenceMutationInfos) {
				var referenceEntry = referenceMutationInfo.Value.TargetEntityMutationInfo.GetEntry();
				referenceMutationInfo.Value.Navigation.PropertyInfo.SetValue(entry.Entity, referenceEntry.Entity);
			};
			foreach (var collectionMutationInfo in CollectionMutationInfos) {
				var collectionEntries = collectionMutationInfo.Value.TargetEntityMutationInfos.Select(targetEntityMutationInfo => targetEntityMutationInfo.GetEntry()).ToArray();
				var collectionAccessor = collectionMutationInfo.Value.Navigation.GetCollectionAccessor()!;
				foreach (var collectionEntry in collectionEntries) {
					collectionAccessor.Add(entity, collectionEntry.Entity, false);
				}
			};
			EntityMutationContext.DbContext.ChangeTracker.DetectChanges();
			if (JObject.TryGetValue("$isEdition", out var isEdition) && isEdition.Value<bool>()) {
				entry.State = EntityState.Modified;
			}
			if (JObject.TryGetValue("$isCreation", out var isCreation) && isCreation.Value<bool>()) {
				entry.State = EntityState.Added;
			}
			if (JObject.TryGetValue("$isDeletion", out var isDeletion) && isDeletion.Value<bool>()) {
				entry.State = EntityState.Deleted;
			}
			return entry;
		}

		public Expression BuildEntityPermissionExpression(EntityEntry entityEntry, Expression instanceExpression) {
			var selfPermissionBinding = BuildSelfPermissionMemberBinding(entityEntry, instanceExpression);

			var primaryKeyBindingExpression = PrimaryKey.Properties.Select(primaryKeyProperty =>
				Expression.Bind(
					PermissionEntityType.GetProperty(primaryKeyProperty.Name),
					Expression.MakeMemberAccess(
						instanceExpression,
						primaryKeyProperty.PropertyInfo
					)
				)
			);

			var propertyBindingExpressions = PropertyMutationInfos.Values.Select(propertyMutationInfo =>
				propertyMutationInfo.BuildMemberBindingExpression(
					entityEntry.Property(propertyMutationInfo.Property),
					instanceExpression
				)
			).ToArray();

			var referenceBindingExpressions = ReferenceMutationInfos.Values.Select(referenceMutationInfo =>
				referenceMutationInfo.BuildMemberBindingExpression(
					entityEntry.Reference(referenceMutationInfo.Navigation),
					instanceExpression
				)
			).ToArray();

			var collectionBindingExpressions = CollectionMutationInfos.Values.Select(collectionMutationInfo =>
				collectionMutationInfo.BuildMemberBindingExpression(
					entityEntry.Collection(collectionMutationInfo.Navigation),
					instanceExpression
				)
			).ToArray();

			var bindings = primaryKeyBindingExpression
				.Append(selfPermissionBinding)
				.Concat(propertyBindingExpressions)
				.Concat(referenceBindingExpressions)
				.Concat(collectionBindingExpressions)
			.ToArray();
			var newPermissionExpression = BuildNewPermissionMemberInitExpression(bindings);

			return newPermissionExpression;
		}
		protected MemberInitExpression BuildNewPermissionMemberInitExpression(MemberBinding[] memberBindings) {
			var newExpression = Expression.New(PermissionEntityType.GetConstructor(new Type[0]));
			return Expression.MemberInit(newExpression, memberBindings);
		}
		public MemberBinding BuildSelfPermissionMemberBinding(EntityEntry entityEntry, Expression instanceExpression) {
			var permissionExpression = RuleMap.GetOperationResolver(entityEntry.State.ToOperation(), EntityMutationContext.SecurityContext, instanceExpression);
			var memberBinding = Expression.Bind(SelfPermissionProperty, permissionExpression);
			return memberBinding;
		}
		public void CheckPermissions(EntityEntry entry, object context, object permissionEntity, ISet<EntityMutationInfo> cache) {
			if (!(IsReference || cache.Contains(this))) {
				cache.Add(this);
				var permissionEntityType = permissionEntity?.GetType();
				var primaryKeyPairs = PropertyMutationInfos.Values.Where(propertyMutationInfo => PrimaryKey.Properties.Any(primaryKey => primaryKey == propertyMutationInfo.Property));
				var primaryKeyTuples = PrimaryKey.Properties.Join(PropertyMutationInfos.Values, primaryKey => primaryKey, propertyMutationInfos => propertyMutationInfos.Property, (primaryKey, pt) => pt);

				var requiredPermission = entry.State.ToOperation();
				var instancePermission = (bool)permissionEntityType?.GetProperty("SelfPermission")?.GetValue(permissionEntity);

				if (!instancePermission) {
					throw new UnauthorizedAccessException($"You do not have permission to {Enum.GetName(typeof(Operation), requiredPermission)} on entity type {EntityType.Name} with id with key ({string.Join(", ", primaryKeyPairs.Select(kp => $"{kp.Property.Name}={kp.JProperty.Value}"))}).");
				}

				foreach (var primaryKeyPropertyTuple in primaryKeyTuples) {
					var instancePropertyPermission = (bool)permissionEntityType.GetProperty($"{primaryKeyPropertyTuple.Property.Name}Permission").GetValue(permissionEntity);
					if (!instancePropertyPermission) {
						throw new UnauthorizedAccessException($"You do not have permission to {Enum.GetName(typeof(Operation), Operation.Read)} on property {primaryKeyPropertyTuple.Property.Name} on entity type {EntityType.Name}.");
					}
				}

				var properties = PropertyMutationInfos.Values.Select(propertyMutationInfo => propertyMutationInfo.Property)
					.Concat(ReferenceMutationInfos.Values.Select(referenceMutationInfo => referenceMutationInfo.Property))
					.Concat(CollectionMutationInfos.Values.Select(collectionMutationInfo => collectionMutationInfo.Property));

				foreach (var property in properties) {
					var instancePropertyPermission = (bool?)permissionEntityType?.GetProperty($"{property.Name}Permission")?.GetValue(permissionEntity);
					if (instancePropertyPermission != null && !instancePropertyPermission.Value) {
						throw new UnauthorizedAccessException($"You do not have permission to {Enum.GetName(typeof(Operation), requiredPermission)} on property {property.Name} on entity type {EntityType.Name} with key ({string.Join(", ", primaryKeyPairs.Select(kp => $"{kp.Property.Name}={kp.JProperty.Value}"))}).");
					}
				}

				foreach (var reference in ReferenceMutationInfos.Values.Where(reference => reference.TargetEntityMutationInfo != null)) {
					reference.TargetEntityMutationInfo.CheckPermissions(entry.Reference(reference.Navigation.Name).EntityEntry, context, permissionEntityType?.GetProperty(reference.Navigation.Name)?.GetValue(permissionEntity), cache);
				}

				foreach (var collectionMutationInfo in CollectionMutationInfos.Values) {
					if (permissionEntity != null) {
						var collection = (object[])PermissionEntityType.GetProperty(collectionMutationInfo.Navigation.Name).GetValue(permissionEntity);
						if (collection != null) {
							var index = 0;
							foreach (var itemMutationInfo in collectionMutationInfo.TargetEntityMutationInfos) {
								if (itemMutationInfo.EntityState != EntityState.Added) {
									var collectionEntry = entry.Collection(collectionMutationInfo.Navigation.Name);
									var itemEntry = collectionEntry.FindEntry(collectionEntry.CurrentValue.Cast<object>().ElementAt(index));
									itemMutationInfo.CheckPermissions(itemEntry, context, collection[index], cache);
									index++;
								}
							}
						}
					}
				}
			}
		}
		public static IQueryable BuildFindEntityQuery(DbContext applicationDbContext, IEntityType entityType, object entity) {
			var query = applicationDbContext.GetQuery(entityType.ClrType);
			var keyPredicateLambdaExpression = GetKeyFilterExpression(entityType, entity);
			//var whereCallExpression = Expression.Call(null, WhereMethodInfo.MakeGenericMethod(entityType.ClrType), HasRootQuery ? query.Expression : Expression.Constant(query), keyPredicateLambdaExpression);
			var whereCallExpression = (IQueryable)WhereMethodInfo.MakeGenericMethod(entityType.ClrType).Invoke(null, new object[] { query, keyPredicateLambdaExpression });
			return whereCallExpression;
		}
		public static Expression BuildFindEntityQuery(Expression queryExpression, IEntityType entityType, object entity) {
			var keyPredicateLambdaExpression = GetKeyFilterExpression(entityType, entity);
			//var whereCallExpression = Expression.Call(null, WhereMethodInfo.MakeGenericMethod(entityType.ClrType), HasRootQuery ? query.Expression : Expression.Constant(query), keyPredicateLambdaExpression);
			var whereCallExpression = Expression.Call(null, CollectionWhereMethodInfo.MakeGenericMethod(entityType.ClrType), queryExpression, keyPredicateLambdaExpression);
			return whereCallExpression;
		}

		public static Expression BuildSelectFirstOrDefaultPermissionEntityExpression(IEntityType entityType, Expression queryableExpression, ParameterExpression parameterExpression, Expression selectBodyExpression) {
			var selectLambdaExpression = Expression.Lambda(selectBodyExpression, parameterExpression);
			var selectMethodInfo = default(MethodInfo);
			var firstOrDefaultMethodInfo = default(MethodInfo);
			if (queryableExpression.Type.GetInterfaces().Any(@interface => @interface == typeof(IQueryable))) {
				selectMethodInfo = SelectMethodInfo;
				firstOrDefaultMethodInfo = FirstOrDefaultMethodInfo;
			} else {
				selectMethodInfo = CollectionSelectMethodInfo;
				firstOrDefaultMethodInfo = CollectionFirstOrDefaultMethodInfo;
			}
			var selectCallExpression = Expression.Call(selectMethodInfo.MakeGenericMethod(entityType.ClrType, selectBodyExpression.Type), queryableExpression, selectLambdaExpression);
			var firstOrDefaultCallExpression = Expression.Call(null, firstOrDefaultMethodInfo.MakeGenericMethod(selectBodyExpression.Type), selectCallExpression);
			return firstOrDefaultCallExpression;
		}

		public static Expression GetKeyFilterExpression(IEntityType entityType, object entity) {
			var parameter = Expression.Parameter(entityType.ClrType);
			var primaryKey = entityType.FindPrimaryKey();
			var predicate = primaryKey.Properties.Select(primaryKeyProperty =>
				Expression.Equal(
					Expression.Property(parameter, primaryKeyProperty.PropertyInfo),
					Expression.Constant(primaryKeyProperty.GetGetter().GetClrValue(entity))
				)
			).Join((keyPredicate, partialPredicate) => Expression.AndAlso(keyPredicate, partialPredicate));
			return Expression.Lambda(predicate, parameter);
		}
	}
}
