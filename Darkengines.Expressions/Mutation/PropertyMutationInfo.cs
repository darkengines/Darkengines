using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public class PropertyMutationInfo : IMemberMutationInfo {
		public IProperty Property { get; }
		public JProperty JProperty { get; }
		IPropertyBase IMemberMutationInfo.Property { get { return Property; } }
		public EntityMutationInfo EntityMutationInfo { get; }
		public PropertyMutationInfo(EntityMutationInfo entityMutationInfo, IProperty property, JProperty jProperty) {
			EntityMutationInfo = entityMutationInfo;
			Property = property;
			JProperty = jProperty;
		}
		public PropertyEntry ProcessPropertyEntry(PropertyEntry propertyEntry) {
			if (!Property.IsPrimaryKey()) {
				propertyEntry.IsModified = true;
			}
			return propertyEntry;
		}
		MemberEntry IMemberMutationInfo.ProcessMemberEntry(MemberEntry memberEntry) { return ProcessPropertyEntry((PropertyEntry)memberEntry); }
		public MemberBinding BuildMemberBindingExpression(MemberEntry memberEntry, Expression instanceExpression) {
			var type = Property.ClrType;
			var permissionPropertyInfo = EntityMutationInfo.PermissionEntityType.GetProperty(Property.Name);
			var resolverExpression = EntityMutationInfo.RuleMap.GetPropertyOperationResolver(
				memberEntry.Metadata.PropertyInfo,
				memberEntry.IsModified ? Operation.Edit : Operation.Read,
				Expression.Constant(EntityMutationInfo.EntityMutationContext.SecurityContext),
				instanceExpression
			);
			//if (resolverExpression is NonQueryExpression nonQueryExpression) {
			//	var permission = nonQueryExpression.Evaluate<bool>();

			//	permissionPropertyInfo.SetValue(EntityMutationInfo.LocalPermissionEntity, permission);
			//}
			return Expression.Bind(EntityMutationInfo.PermissionEntityType.GetProperty($"{Property.Name}Permission"), resolverExpression);
		}
	}
}
