using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public class PropertyRuleMap<TItem, TProperty, TContext> : IPropertyRuleMap {
		public PropertyInfo PropertyInfo { get; }
		protected IDictionary<object, LambdaExpression> Resolvers { get; }

		public PropertyRuleMap(PropertyInfo propertyInfo) : base() {
			PropertyInfo = propertyInfo;
			Resolvers = new Dictionary<object, LambdaExpression>();
		}

		public PropertyRuleMap<TItem, TProperty, TContext> WithResolver<TResult>(object key, Expression<PropertyResolver<TItem, TProperty, TContext, TResult>> resolver) {
			Resolvers[key] = resolver;
			return this;
		}
		public PropertyRuleMap<TItem, TProperty, TContext> WithOperation(Operation operation) {
			return WithResolver(operation, (instance, context) => true);
		}
		public PropertyRuleMap<TItem, TProperty, TContext> WithOperation(Operation operation, Expression<PropertyResolver<TItem, TProperty, TContext, bool>> operationResolver) {
			return WithResolver(operation, operationResolver);
		}
		public Expression GetResolver(object key, Expression contextExpression, Expression instanceExpression) {
			var resolverBody = (Expression)new NonQueryExpression(Expression.Constant(false));
			if (Resolvers.TryGetValue(key, out var resolver)) {
				resolverBody = resolver.Body;
				resolverBody = resolverBody.Replace(new Dictionary<Expression, Expression>() {
					{ resolver.Parameters[0], instanceExpression },
					{ resolver.Parameters[1], contextExpression }
				});
				var booleanExpressionVisitor = new NonQueryExpressionVisitor();
				resolverBody = booleanExpressionVisitor.Visit(resolverBody);
			}
			return resolverBody;
		}
		public Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(operation, contextExpression, instanceExpression);
		}

		Expression IPropertyRuleMap.GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetOperationResolver(operation, contextExpression, instanceExpression);
		}

		Expression IPropertyRuleMap.GetResolver(object key, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(key, contextExpression, instanceExpression);
		}
	}
}
