using Darkengines.Expressions.Application.Users.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Users.Rules {
	public class UserRule : TypeRuleMap<User, ApplicationContext> {
		public UserRule() {
			WithOperation(
				Operation.Read,
				(instance, context) => instance.UserUserGroups.Any(leftUserUserGroup =>
					leftUserUserGroup.UserGroup.UserUserGroups.Any(rightUserUserGroup =>
						rightUserUserGroup.UserId == context.CurrentUser.Id
					)
				)
			);
			Property(user => user.EmailAddress!).WithOperation(
				Operation.ReadWrite,
				(instance, context) => instance.Id == context.CurrentUser.Id
			);
		}
	}
}
