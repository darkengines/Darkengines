using Darkengines.Applications;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Users.Rules {
	public class UserProfileRule : TypeRuleMap<UserProfile, IApplicationContext> {
		public UserProfileRule() {
			WithOperation(
				Operation.ReadWrite,
				(instance, context) => instance.Id == context.CurrentUser.Id
			);
			Expose(userProfile => userProfile.Id).WithOperation(Operation.Read);
			Expose(userProfile => userProfile.DisplayName).WithOperation(Operation.ReadWrite);
			Expose(userProfile => userProfile.Firstname).WithOperation(Operation.ReadWrite);
			Expose(userProfile => userProfile.Lastname).WithOperation(Operation.ReadWrite);
			Expose(userProfile => userProfile.ImageUri).WithOperation(Operation.ReadWrite);
			Expose(userProfile => userProfile.Gender).WithOperation(Operation.ReadWrite);
		}
	}
}
