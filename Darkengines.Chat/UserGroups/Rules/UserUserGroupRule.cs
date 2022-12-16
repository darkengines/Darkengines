using Darkengines.Expressions.Application.UserGroups.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Applications.Rules {
	public class UserUserGroupRule : TypeRuleMap<UserUserGroup, ApplicationContext> {
		public UserUserGroupRule() {
			Property(userUserGroup => userUserGroup.UserId!).WithOperation(Operation.Read);
			Property(userUserGroup => userUserGroup.UserGroupId!).WithOperation(Operation.Read);
			Property(userUserGroup => userUserGroup.User!).WithOperation(Operation.Read);
			Property(userUserGroup => userUserGroup.UserGroup!).WithOperation(Operation.Read);
		}
	}
}
