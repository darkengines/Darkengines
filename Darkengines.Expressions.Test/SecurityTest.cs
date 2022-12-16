using Darkengines.Core.Applications;
using Darkengines.Core.Users.Entities;
using Darkengines.Core.Users.Rules;
using Darkengines.Expressions.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Test {
	[TestClass]
	public class SecurityTest {
		[TestMethod]
		public void TestBooleanExpressionVisitor() {
			var currentUser = new User() {
				Id = 0,
				UserProfile = new UserProfile() {
					DisplayName = "User#0",
				}
			};
			var context = new ApplicationContext(currentUser);
			var userRule = new UserRule();
			var parameter = Expression.Parameter(typeof(User));
			var resolver = userRule.GetOperationResolver(Operation.Read, context, parameter);
			if (resolver != null) {
				var booleanExpressionVisitor = new BooleanExpressionVisitor();
				var result = booleanExpressionVisitor.Visit(resolver);
			}
		}
	}
}
