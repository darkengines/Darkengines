using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Darkengines.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public abstract class RuleMap<TItem, TContext> : IRuleMap {
		public Type Type { get { return typeof(TItem); } }
		protected IDictionary<object, LambdaExpression> Resolvers { get; }
		public RuleMap() {
			Resolvers = new Dictionary<object, LambdaExpression>();
		}
		public RuleMap<TItem, TContext> WithResolver<TResult>(object key, Expression<Resolver<TItem, TContext, TResult>> resolver) {
			Resolvers[key] = resolver;
			return this;
		}
		public RuleMap<TItem, TContext> WithOperation(Operation operation) {
			return WithResolver(operation, (instance, context) => true);
		}
		public RuleMap<TItem, TContext> WithOperation(Operation operation, Expression<Resolver<TItem, TContext, bool>> operatioResolver) {
			return WithResolver(operation, operatioResolver);
		}
		public Expression GetResolver(object key, Expression contextExpression, Expression instanceExpression) {
			var resolverBody = (Expression)new NonQueryExpression(Expression.Constant(false));
			if (Resolvers.TryGetValue(key, out var resolver)) {
				resolverBody = resolver.Body;
				resolverBody = resolverBody.Replace(new Dictionary<Expression, Expression>() {
					{ resolver.Parameters[0], instanceExpression },
					{ resolver.Parameters[1], Expression.Constant(contextExpression) }
				});
				var booleanExpressionVisitor = new NonQueryExpressionVisitor();
				resolverBody = booleanExpressionVisitor.Visit(resolverBody);
			}
			return resolverBody;
		}
		public Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(operation, contextExpression, instanceExpression);
		}

		Expression IRuleMap.GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetOperationResolver(operation, contextExpression, instanceExpression);
		}

		Expression IRuleMap.GetResolver(object key, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(key, contextExpression, instanceExpression);
		}

		public abstract Expression? GetPropertyResolver(PropertyInfo propertyInfo, object key, Expression contextExpression, Expression instanceExpression);
		public abstract Expression? GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression contextExpression, Expression instanceExpression);
		Expression? IRuleMap.GetPropertyResolver(PropertyInfo propertyInfo, object key, Expression contextExpression, Expression instanceExpression) {
			return GetPropertyResolver(propertyInfo, key, contextExpression, instanceExpression);
		}

		Expression? IRuleMap.GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetPropertyOperationResolver(propertyInfo, operation, contextExpression, instanceExpression);
		}
	}
}
