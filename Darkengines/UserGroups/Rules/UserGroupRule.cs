using Darkengines.Applications;
using Darkengines.UserGroups.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.UserGroups.Rules {
	public class UserGroupRule : TypeRuleMap<UserGroup, IApplicationContext> {
		public UserGroupRule() {
			WithOperation(Operation.Read | Operation.Create);
			Expose(userGroup => userGroup.Id!).WithOperation(Operation.Read);
			Expose(userGroup => userGroup.DisplayName!).WithOperation(Operation.ReadWrite);
			Expose(userGroup => userGroup.UserUserGroups!).WithOperation(Operation.ReadWrite);
		}
	}
}
