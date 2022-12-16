using Darkengines.Core.Applications;
using Darkengines.Core.Users.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Core.Users.Rules {
	public class UserRule : TypeRuleMap<User, IApplicationContext> {
		public UserRule() {
			WithOperation(
				Operation.Read,
				(instance, context) => instance.UserUserGroups.Any(leftUserUserGroup =>
					leftUserUserGroup.UserGroup.UserUserGroups.Any(rightUserUserGroup =>
						rightUserUserGroup.UserId == context.CurrentUser.Id
					)
				)
			);
			Expose(user => user.UserEmailAddresses).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
			Expose(user => user.UserEmailAddresses).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
			Expose(user => user.HashedPassword).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
			//Property(user => user.EmailAddress!).WithOperation(
			//	Operation.ReadWrite,
			//	(instance, context) => instance.Id == context.CurrentUser.Id
			//);
		}
	}
}
