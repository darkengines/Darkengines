using Darkengines.UserGroups.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Applications.Rules {
	public class UserUserGroupRule : TypeRuleMap<UserUserGroup, IApplicationContext> {
		public UserUserGroupRule() {
			WithOperation(Operation.Create);
			Expose(userUserGroup => userUserGroup.UserId!).WithOperation(Operation.Read | Operation.Create);
			Expose(userUserGroup => userUserGroup.Index).WithOperation(Operation.Read | Operation.Create);
			Expose(userUserGroup => userUserGroup.UserGroupId!).WithOperation(Operation.Read | Operation.Create);
			Expose(userUserGroup => userUserGroup.User!).WithOperation(Operation.Read | Operation.Create);
			Expose(userUserGroup => userUserGroup.UserGroup!).WithOperation(Operation.Read | Operation.Create);
		}
	}
}
