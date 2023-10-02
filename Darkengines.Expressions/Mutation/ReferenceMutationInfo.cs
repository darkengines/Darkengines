using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Darkengines.Expressions.Mutation {
    public class ReferenceMutationInfo : NavigationMutationInfo {
        public EntityMutationInfo TargetEntityMutationInfo { get; }
        public ReferenceMutationInfo(EntityMutationInfo entityMutationInfo, INavigation navigation, JProperty jProperty) : base(entityMutationInfo, navigation, jProperty) {
            var jObject = (JObject)JProperty.Value;
            if (jObject.TryGetValue("$ref", out var jObjectReferenceToken)) {
                var jObjectReference = jObjectReferenceToken.Value<string>()!;
                TargetEntityMutationInfo = entityMutationInfo.EntityMutationContext.JObjectEntityMutationInfoMapping[EntityMutationInfo.EntityMutationContext.JObjectReferenceMapping[jObjectReference]];
            } else {
                TargetEntityMutationInfo = new EntityMutationInfo(navigation.TargetEntityType, jObject, EntityMutationInfo.EntityMutationContext);
            }
            //Navigation.PropertyInfo!.SetValue(EntityMutationInfo.Entity, TargetEntityMutationInfo.Entity);
        }

        public override NavigationEntry ProcessNavigationEntry(NavigationEntry navigationEntry) {
            return ProcessReferenceEntry((ReferenceEntry)navigationEntry);
        }
        public ReferenceEntry ProcessReferenceEntry(ReferenceEntry referenceEntry) {

            return referenceEntry;
        }

        public override MemberBinding BuildMemberBindingExpression(MemberEntry memberEntry, Expression instanceExpression) {
            var propertyInfo = Navigation.PropertyInfo;
            var permissionPropertyInfo = EntityMutationInfo.PermissionEntityType.GetProperty(Property.Name);
            var memberExpression = Expression.MakeMemberAccess(instanceExpression, propertyInfo);
            var resolverExpression = TargetEntityMutationInfo.RuleMap.GetOperationResolver(
                memberEntry.IsModified ? Operation.Write : Operation.Read,
                EntityMutationInfo.EntityMutationContext.SecurityContext,
                instanceExpression
            );
            return Expression.Bind(EntityMutationInfo.PermissionEntityType.GetProperty($"{propertyInfo.Name}Permission"), resolverExpression);

        }
    }
}
