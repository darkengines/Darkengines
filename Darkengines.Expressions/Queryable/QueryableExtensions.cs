using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Queryable {
	public static class QueryableExtensions {
		public static MethodInfo WithProjectionGenericMethodInfo = ExpressionHelper.ExtractGenericDefinitionMethodInfo<IQueryable<object>, Func<Expression<Func<object, object>>, IQueryable<object>>>(queryable => queryable.WithProjection);
		public static IQueryable<TEntity> WithProjection<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, TEntity>> selector) where TEntity : class {
			var methodInfo = WithProjectionGenericMethodInfo.MakeGenericMethod(typeof(TEntity));
			var methodCallExpression = Expression.Call(methodInfo, source.Expression, selector);
			return source.Provider.CreateQuery<TEntity>(methodCallExpression);
		}
	}
}
