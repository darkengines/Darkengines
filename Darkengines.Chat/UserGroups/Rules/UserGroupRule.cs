using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkengines.Expressions.Application.UserGroups.Entities;
using Darkengines.Expressions.Application;

namespace Darkengines.Expressions.userGroup.userGroups.Rules {
	public class UserGroupRule : TypeRuleMap<UserGroup, ApplicationContext> {
		public UserGroupRule() {
			Property(userGroup => userGroup.Id!).WithOperation(Operation.Read);
			Property(userGroup => userGroup.DisplayName!).WithOperation(Operation.Read);
			Property(userGroup => userGroup.UserUserGroups!).WithOperation(Operation.Read);
		}
	}
}
