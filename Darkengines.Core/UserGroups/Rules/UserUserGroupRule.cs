using Darkengines.Core.UserGroups.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Applications.Rules
{
	public class UserUserGroupRule : TypeRuleMap<UserUserGroup, IApplicationContext> {
		public UserUserGroupRule() {
			Expose(userUserGroup => userUserGroup.UserId!).WithOperation(Operation.Read);
			Expose(userUserGroup => userUserGroup.UserGroupId!).WithOperation(Operation.Read);
			Expose(userUserGroup => userUserGroup.User!).WithOperation(Operation.Read);
			Expose(userUserGroup => userUserGroup.UserGroup!).WithOperation(Operation.Read);
		}
	}
}
