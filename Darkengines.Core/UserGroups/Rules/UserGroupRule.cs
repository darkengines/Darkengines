using Darkengines.Core.Applications;
using Darkengines.Core.UserGroups.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Core.UserGroups.Rules
{
	public class UserGroupRule : TypeRuleMap<UserGroup, IApplicationContext> {
		public UserGroupRule() {
			Expose(userGroup => userGroup.Id!).WithOperation(Operation.Read);
			Expose(userGroup => userGroup.DisplayName!).WithOperation(Operation.Read);
			Expose(userGroup => userGroup.UserUserGroups!).WithOperation(Operation.Read);
		}
	}
}
