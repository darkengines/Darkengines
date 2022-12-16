using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions {
	public static class ExpressionExtensions {
		public static Expression Replace(this Expression expression, IDictionary<Expression, Expression> replacement) {
			var replacementExpressionVisitor = new ReplacementExpressionVisitor(replacement);
			return replacementExpressionVisitor.Visit(expression);
		}
	}
}
