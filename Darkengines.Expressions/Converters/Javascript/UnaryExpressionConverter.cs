using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class UnaryExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.UnaryExpression> {
		public override ConverterResult Convert(Esprima.Ast.UnaryExpression syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var unaryArgument = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Argument).Convert(syntaxNode.Argument, syntaxNodeConverterContext, scope);
			var expression = Expression.MakeUnary(UnaryExpressionMap.ExpressionTypeMap[syntaxNode.Operator], unaryArgument.Expression, unaryArgument.Expression.Type);
			return new ConverterResult(expression);
		}
	}
}
