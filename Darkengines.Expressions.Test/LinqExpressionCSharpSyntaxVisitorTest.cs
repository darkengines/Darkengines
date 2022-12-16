using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using System.Linq;
using Darkengines.Core.Users.Entities;

namespace Darkengines.Core.Test {
	[TestClass]
	public class LinqExpressionCSharpSyntaxVisitorTest {
		protected SyntaxNodeConverterContext Context { get; }
		protected async Task<SyntaxNode> BuildSyntaxNode(string source) {
			var tree = CSharpSyntaxTree.ParseText(source);
			var root = await tree.GetRootAsync();
			return root;
		}
		protected IEnumerable<User> Users { get; set; }
		public LinqExpressionCSharpSyntaxVisitorTest() {
			Users = Enumerable.Range(0, 100).Select(index => new User() {
				Id = index,
				DisplayName = $"Slayer #{index}",
				Firstname = "Guy",
				Lastname = "Doom",
				EmailAddress = $"slayer{index}@uac.com"
			}).ToArray();
			var user = Users.First();
			var identifiers = new Dictionary<string, Expression>() {
				{ "user", Expression.Constant(user) },
				{ "Users", Expression.Constant(Users) }
			};
			var extensionTypes = new[] { typeof(Enumerable), typeof(UserExtensions) };
			var typeIdentifiers = new Dictionary<string, Type>() {
				{ nameof(User), typeof(User) }
			};
			Context = new SyntaxNodeConverterContext() {
				Identifiers = identifiers,
				TypeIdentifiers = typeIdentifiers,
				ExtensionTypes = extensionTypes
			};
		}
		[TestMethod]
		public async Task TestLiterals() {
			var source = "1";
			var loremIpsum = "lorem ipsum";
			var source1 = $@"""{loremIpsum}""";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;
			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
			Assert.AreEqual(((ConstantExpression)expression).Value, 1);

			var root1 = await BuildSyntaxNode(source1);
			var expression1 = Context.Convert(root1).Expression;
			Assert.IsNotNull(expression1);
			Assert.IsInstanceOfType(expression1, typeof(ConstantExpression));
			Assert.AreEqual(((ConstantExpression)expression1).Value, loremIpsum);
		}

		[TestMethod]
		public async Task TestIdentifiers() {
			var source = "user";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.AreEqual(expression.Type, typeof(User));
		}

		[TestMethod]
		public async Task TestMemberAccess() {
			var source = "user.Id";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(MemberExpression));
			var memberExpression = (MemberExpression)expression;
			Assert.AreEqual(memberExpression.Expression!.Type, typeof(User));
			Assert.AreEqual(memberExpression.Member, ExpressionHelper.ExtractPropertyInfo<User>(user => user.Id));
		}

		[TestMethod]
		public async Task TestStaticMemberAccess() {
			var source = "System.DateTime.Now";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(MemberExpression));
		}

		[TestMethod]
		public async Task TestBinaryOperator() {
			var leftValue = 1;
			var rightValue = 1;
			var source = $"{leftValue}+{rightValue}";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(BinaryExpression));
			var binaryExpression = (BinaryExpression)expression;
			Assert.AreEqual(binaryExpression.NodeType, ExpressionType.Add);
		}

		[TestMethod]
		public async Task TestStaticIdentifier() {
			var className = nameof(DateTime);
			var staticMemberName = nameof(DateTime.Now);
			var source = $"System.{className}.{staticMemberName}";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(MemberExpression));
		}

		[TestMethod]
		public async Task TestLambdaExpressions() {
			var source = @"(int x, int y,  int z) => x * x + y * y + z * z";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(Expression<Func<int, int, int, int>>));
		}

		[TestMethod]
		public async Task TestMemberMethodInvocation() {
			var source = @"user.ToString()";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(MethodCallExpression));
		}
		[TestMethod]
		public async Task TestStaticMethodInvocation() {
			var source = $@"System.DateTime.DaysInMonth(1989, 11)";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;

			var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression!, typeof(object)));
			var func = lambda.Compile();

			var result = func();

			Assert.IsNotNull(expression);
			Assert.IsInstanceOfType(expression, typeof(MethodCallExpression));
		}
		[TestMethod]
		public async Task TestExtensionMethodInvocation() {
			var source = $"user.GetDisplayName()";

			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;
			Assert.IsNotNull(root);
			var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression!, typeof(object)));
			var func = lambda.Compile();

			var result = func();
		}
		[TestMethod]
		public async Task TestGenericExtensionMethodInvocation() {
			var source = $"Users.Take(1)";
			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root);
			Assert.IsNotNull(root);
		}
		[TestMethod]
		public async Task TestGenericExtensionMethodWithGenericArgumentsInvocation() {
			var source =
@"Users.GroupJoin(Users.Where(user => user.Id % 2 == 0), user => user.Id,user => user.Id, (user, group) => group)";
			var root = await BuildSyntaxNode(source);
			var expression = Context.Convert(root).Expression;
			Assert.IsNotNull(root);
		}
	}
}