using Darkengines.Core.Applications;
using Darkengines.Core.Users.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Core.Users.Rules {
	public class UserProfileRule : TypeRuleMap<UserProfile, IApplicationContext> {
		public UserProfileRule() {
			WithOperation(
				Operation.Write,
				(instance, context) => instance.Id == context.CurrentUser.Id
			);
		}
	}
}
