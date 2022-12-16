using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public class CollectionMutationInfo : NavigationMutationInfo {
		public HashSet<EntityMutationInfo> TargetEntityMutationInfos { get; }
		public CollectionMutationInfo(EntityMutationInfo entityMutationInfo, INavigation navigation, JProperty jProperty) : base(entityMutationInfo, navigation, jProperty) {
			TargetEntityMutationInfos = JProperty.Values().Select(value => {
				var jObject = (JObject)value;
				if (jObject.TryGetValue("$ref", out var jObjectReferenceToken)) {
					var jObjectReference = jObjectReferenceToken.Value<string>()!;
					return entityMutationInfo.EntityMutationContext.JObjectEntityMutationInfoMapping[EntityMutationInfo.EntityMutationContext.JObjectReferenceMapping[jObjectReference]];
				} else {
					return new EntityMutationInfo(navigation.TargetEntityType, jObject, EntityMutationInfo.EntityMutationContext);
				}
			}).ToHashSet();
			//var collection = Navigation.GetCollectionAccessor()!;
			//foreach (var targetMutationInfo in TargetEntityMutationInfos) {
			//	collection.Add(EntityMutationInfo.Entity, targetMutationInfo.Entity, false);
			//}
		}
		public override NavigationEntry ProcessNavigationEntry(NavigationEntry navigationEntry) {
			return ProcessCollectionEntry((CollectionEntry)navigationEntry);
		}
		public CollectionEntry ProcessCollectionEntry(CollectionEntry collectionEntry) {
			return collectionEntry;
		}
		public override MemberBinding BuildMemberBindingExpression(MemberEntry memberEntry, Expression instanceExpression) { return BuildMemberBindingExpression((CollectionEntry)memberEntry, instanceExpression); }
		public MemberBinding BuildMemberBindingExpression(CollectionEntry collectionEntry, Expression instanceExpression) {
			var values = TargetEntityMutationInfos.Select((entityMutationInfo, index) => {
				var targetEntry = collectionEntry.FindEntry(collectionEntry.CurrentValue.Cast<object>().ElementAt(index));
				var itemParameter = Expression.Parameter(Navigation.TargetEntityType.ClrType);
				var body = entityMutationInfo.BuildEntityPermissionExpression(targetEntry, itemParameter);
				var query = EntityMutationInfo.BuildFindEntityQuery(
					Expression.Property(instanceExpression, Navigation.PropertyInfo),
					Navigation.TargetEntityType,
					targetEntry.Entity
				);
				var result = EntityMutationInfo.BuildSelectFirstOrDefaultPermissionEntityExpression(
					Navigation.TargetEntityType,
					query,
					itemParameter,
					body
				);
				return result;
			}).ToArray();
			if (values.Any()) {
				var permissionCollectionPropertyInfo = EntityMutationInfo.PermissionEntityType.GetProperty($"{Property.Name}");
				var bindingExpression = Expression.Bind(
					permissionCollectionPropertyInfo,
					Expression.NewArrayInit(
						permissionCollectionPropertyInfo.PropertyType.GetEnumerableUnderlyingType(),
						values
					)
				);
				return bindingExpression;
			}
			return null;
		}
	}
}
