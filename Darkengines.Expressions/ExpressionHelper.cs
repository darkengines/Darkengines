using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Darkengines.Expressions {
	public static class ExpressionHelper {
		public static MethodInfo ExtractMethodInfo<TDeclaringType, TMethod>(Expression<Func<TDeclaringType, TMethod>> methodAccessExpression) {
			return (MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodAccessExpression.Body).Operand).Object!).Value!;
		}
		public static MethodInfo ExtractMethodInfo<TMethod>(Expression<Func<TMethod>> methodAccessExpression) {
			return (MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodAccessExpression.Body).Operand).Object!).Value!;
		}
		public static MethodInfo ExtractGenericDefinitionMethodInfo<TDeclaringType, TMethod>(Expression<Func<TDeclaringType, TMethod>> methodAccessExpression) {
			return ((MethodInfo)((ConstantExpression)((MethodCallExpression)((UnaryExpression)methodAccessExpression.Body).Operand).Object!).Value!).GetGenericMethodDefinition();
		}
		public static PropertyInfo ExtractPropertyInfo<TDeclaringType>(Expression<Func<TDeclaringType, object>> propertyAccessExpression) {
			return ExtractPropertyInfo(propertyAccessExpression);
		}
		public static PropertyInfo ExtractPropertyInfo<TDeclaringType, TProperty>(Expression<Func<TDeclaringType, TProperty>> propertyAccessExpression) {
			var unaryExpression = propertyAccessExpression.Body as UnaryExpression;
			if (unaryExpression != null) {
				return (PropertyInfo)((MemberExpression)unaryExpression.Operand).Member;
			}
			return (PropertyInfo)((MemberExpression)propertyAccessExpression.Body).Member;
		}
		public static Expression Join(this IEnumerable<Expression> expressions, Func<Expression, Expression, Expression> reducer) {
			return expressions.Skip(1).Any() ? expressions.Skip(1).Aggregate(expressions.First(), reducer) : expressions.First();
		}
		public static TSource Reduce<TSource>(this TSource source, Func<TSource, TSource> reducer, Func<TSource, bool> predicate) {
			while (predicate(source)) {
				source = reducer(source);
			}
			return source;
		}
	}
}
