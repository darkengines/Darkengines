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
			for (var flagIndex = 0; flagIndex < 32; flagIndex++) {
				var flag = (Operation)(1 << flagIndex);
				if ((operation & flag) == flag) {
					WithResolver(flag, (context, instance) => true);
				}
			}
			return this;
		}
		public PropertyRuleMap<TItem, TProperty, TContext> WithOperation(Operation operation, Expression<PropertyResolver<TItem, TProperty, TContext, bool>> operationResolver) {
			for (var flagIndex = 0; flagIndex < 32; flagIndex++) {
				var flag = (Operation)(1 << flagIndex);
				if ((operation & flag) == flag) {
					WithResolver(flag, operationResolver);
				}
			}
			return this;
		}
		public bool TryGetResolver(object key, Expression contextExpression, Expression instanceExpression, out Expression? resolver) {
			if (Resolvers.TryGetValue(key, out var resolverLambdaExpression)) {
				var resolverBody = resolverLambdaExpression.Body;
				resolverBody = resolverBody.Replace(new Dictionary<Expression, Expression>() {
					{ resolverLambdaExpression.Parameters[0], contextExpression },
					{ resolverLambdaExpression.Parameters[1], instanceExpression }
				});
				var booleanExpressionVisitor = new NonQueryExpressionVisitor();
				resolver = booleanExpressionVisitor.Visit(resolverBody);
				return true;
			}
			resolver = default;
			return false;
		}
		public Expression GetResolver(object key, Expression contextExpression, Expression instanceExpression) {
			if (TryGetResolver(key, contextExpression, instanceExpression, out var resolver)) {
				return resolver;
			}
			throw new NotSupportedException($"No resolver found for key {key} on {PropertyInfo.DeclaringType?.Name ?? "<No declaring type>"}.{PropertyInfo.Name}.");
		}
		public bool TryGetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression, out Expression? resolver) {
			return TryGetResolver(operation, contextExpression, instanceExpression, out resolver);
		}
		public Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(operation, contextExpression, instanceExpression);
		}
	}
}
