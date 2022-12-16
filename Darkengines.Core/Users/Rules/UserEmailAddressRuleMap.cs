using Darkengines.Core.Applications;
using Darkengines.Core.Users.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Users.Rules {
	public class UserEmailAddressRuleMap : TypeRuleMap<UserEmailAddress, IApplicationContext> {
		public UserEmailAddressRuleMap() {
			WithOperation(Operation.Write);
			Expose(userEmailAddress => userEmailAddress.EmailAddress).WithResolver(Operation.ReadBackExpression, (userEmailAddress, application) => userEmailAddress.UserId == application.CurrentUser.Id);
			Expose(userEmailAddress => userEmailAddress.HashedEmailAddress).WithResolver(Operation.ReadBackExpression, (userEmailAddress, application) => userEmailAddress.UserId == application.CurrentUser.Id);
			Expose(userEmailAddress => userEmailAddress.Guid).WithResolver(Operation.ReadBackExpression, (userEmailAddress, application) => Expression.Constant(default(Guid?)));
		}
	}
}
