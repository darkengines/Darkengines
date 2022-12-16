using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public static class BinaryExpressionMap {
		public static IDictionary<BinaryOperator, ExpressionType> ExpressionTypeMap = new Dictionary<BinaryOperator, ExpressionType> {
			{ BinaryOperator.Plus, ExpressionType.Add },
			{ BinaryOperator.Minus, ExpressionType.Subtract },
			{ BinaryOperator.Times,ExpressionType.Multiply },
			{ BinaryOperator.Divide,ExpressionType.Divide },
			{ BinaryOperator.Modulo,ExpressionType.Modulo },
			{ BinaryOperator.LeftShift,ExpressionType.LeftShift },
			{ BinaryOperator.RightShift,ExpressionType.RightShift },
			{ BinaryOperator.LogicalOr,ExpressionType.OrElse },
			{ BinaryOperator.LogicalAnd,ExpressionType.AndAlso },
			{ BinaryOperator.BitwiseOr,ExpressionType.Or },
			{ BinaryOperator.BitwiseAnd,ExpressionType.And },
			{ BinaryOperator.Equal,ExpressionType.Equal },
			{ BinaryOperator.NotEqual,ExpressionType.NotEqual },
			{ BinaryOperator.Less,ExpressionType.LessThan },
			{ BinaryOperator.LessOrEqual,ExpressionType.LessThanOrEqual },
			{ BinaryOperator.Greater,ExpressionType.GreaterThan },
			{ BinaryOperator.GreaterOrEqual,ExpressionType.GreaterThanOrEqual },
		};
	}
}
