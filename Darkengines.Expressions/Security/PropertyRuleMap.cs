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
		public PropertyRuleMap<TItem, TProperty, TContext> WithOperation(Operation operation, Expression<PropertyResolver<TItem, TProperty, TContext, bool>> operatioResolver) {
			return WithResolver(operation, operatioResolver);
		}
		public Expression GetResolver(object key, TContext context, Expression instanceExpression) {
			var resolverBody = (Expression)new NonQueryExpression(Expression.Constant(false));
			if (Resolvers.TryGetValue(key, out var resolver)) {
				resolverBody = resolver.Body;
				resolverBody = resolverBody.Replace(new Dictionary<Expression, Expression>() {
					{ resolver.Parameters[0], instanceExpression },
					{ resolver.Parameters[1], Expression.Constant(context) }
				});
				var booleanExpressionVisitor = new NonQueryExpressionVisitor();
				resolverBody = booleanExpressionVisitor.Visit(resolverBody);
			}
			return resolverBody;
		}
		public Expression GetOperationResolver(Operation operation, TContext context, Expression instanceExpression) {
			return GetResolver(operation, context, instanceExpression);
		}

		Expression IPropertyRuleMap.GetOperationResolver(Operation operation, object context, Expression instanceExpression) {
			return GetOperationResolver(operation, (TContext)context, instanceExpression);
		}

		Expression IPropertyRuleMap.GetResolver(object key, object context, Expression instanceExpression) {
			return GetResolver(key, (TContext)context, instanceExpression);
		}
	}
}
