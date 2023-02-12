using Darkengines.Applications;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Users.Rules {
	public class UserProfileRule : TypeRuleMap<UserProfile, IApplicationContext> {
		public UserProfileRule() {
			WithOperation(
				Operation.Write,
				(instance, context) => instance.Id == context.CurrentUser.Id
			);
		}
	}
}
