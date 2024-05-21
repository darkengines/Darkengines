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
			for (var flagIndex = 0; flagIndex < 32; flagIndex++) {
				var flag = (Operation)(1 << flagIndex);
				if ((operation & flag) == flag) {
					WithResolver(flag, (instance, context) => true);
				}
			}
			return this;
		}
		public RuleMap<TItem, TContext> WithOperation(Operation operation, Expression<Resolver<TItem, TContext, bool>> operatioResolver) {
			for (var flagIndex = 0; flagIndex < 32; flagIndex++) {
				var flag = (Operation)(1 << flagIndex);
				if ((operation & flag) == flag) {
					WithResolver(flag, operatioResolver);
				}
			}
			return this;
		}
		public bool TryGetResolver(object key, Expression contextExpression, Expression instanceExpression, out Expression? resolver) {
			if (Resolvers.TryGetValue(key, out var resolverLambdaExpression)) {
				var resolverBody = resolverLambdaExpression.Body;
				resolverBody = resolverBody.Replace(new Dictionary<Expression, Expression>() {
					{ resolverLambdaExpression.Parameters[0], instanceExpression },
					{ resolverLambdaExpression.Parameters[1], contextExpression }
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
			throw new NotSupportedException($"No resolver found for key {key} on entity type {Type}");
		}
		public bool TryGetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression, out Expression? resolver) {
			return TryGetResolver(operation, contextExpression, instanceExpression, out resolver);
		}
		public Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetResolver(operation, contextExpression, instanceExpression);
		}

		public Expression GetPropertyResolver(PropertyInfo propertyInfo, object key, Expression contextExpression, Expression instanceExpression) {
			if (TryGetPropertyResolver(propertyInfo, key, contextExpression, instanceExpression, out instanceExpression)) return instanceExpression;
			throw new NotSupportedException($"No resolver found for key {key} on {propertyInfo.DeclaringType.Name ?? "<DeclaringType>"}.{propertyInfo.Name}");
		}
		public Expression GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression contextExpression, Expression instanceExpression) {
			return GetPropertyResolver(propertyInfo, operation, contextExpression, instanceExpression);
		}
		public abstract bool TryGetPropertyResolver(PropertyInfo propertyInfo, object key, Expression contextExpression, Expression instanceExpression, out Expression? resolver);
		public abstract bool TryGetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression contextExpression, Expression instanceExpression, out Expression? resolver);
	}
}
