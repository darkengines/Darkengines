using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public static class UnaryExpressionMap {
		public static IDictionary<UnaryOperator, ExpressionType> ExpressionTypeMap = new Dictionary<UnaryOperator, ExpressionType> {
			{ UnaryOperator.LogicalNot, ExpressionType.Not },
		};
	}
}
