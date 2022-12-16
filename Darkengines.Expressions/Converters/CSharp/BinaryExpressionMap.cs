using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public static class BinaryExpressionMap {
		public static IDictionary<SyntaxKind, ExpressionType> ExpressionTypeMap = new Dictionary<SyntaxKind, ExpressionType> {
			{ SyntaxKind.AddExpression, ExpressionType.Add },
			{ SyntaxKind.SubtractExpression, ExpressionType.Subtract },
			{ SyntaxKind.MultiplyExpression,ExpressionType.Multiply },
			{ SyntaxKind.DivideExpression,ExpressionType.Divide },
			{ SyntaxKind.ModuloExpression,ExpressionType.Modulo },
			{ SyntaxKind.LeftShiftExpression,ExpressionType.LeftShift },
			{ SyntaxKind.RightShiftExpression,ExpressionType.RightShift },
			{ SyntaxKind.LogicalOrExpression,ExpressionType.OrElse },
			{ SyntaxKind.LogicalAndExpression,ExpressionType.AndAlso },
			{ SyntaxKind.BitwiseOrExpression,ExpressionType.Or },
			{ SyntaxKind.BitwiseAndExpression,ExpressionType.And },
			{ SyntaxKind.ExclusiveOrExpression,ExpressionType.ExclusiveOr },
			{ SyntaxKind.EqualsExpression,ExpressionType.Equal },
			{ SyntaxKind.NotEqualsExpression,ExpressionType.NotEqual },
			{ SyntaxKind.LessThanExpression,ExpressionType.LessThan },
			{ SyntaxKind.LessThanOrEqualExpression,ExpressionType.LessThanOrEqual },
			{ SyntaxKind.GreaterThanExpression,ExpressionType.GreaterThan },
			{ SyntaxKind.GreaterThanOrEqualExpression,ExpressionType.GreaterThanOrEqual },
			{ SyntaxKind.IsExpression,ExpressionType.TypeIs },
			{ SyntaxKind.AsExpression,ExpressionType.TypeAs },
			{ SyntaxKind.CoalesceExpression,ExpressionType.Coalesce }
		};
	}
}
