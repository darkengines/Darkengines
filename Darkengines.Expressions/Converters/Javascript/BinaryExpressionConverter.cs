using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class BinaryExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.BinaryExpression> {
		public override ConverterResult Convert(Esprima.Ast.BinaryExpression syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var left = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Left).Convert(syntaxNode.Left, syntaxNodeConverterContext, scope);
			var right = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Right).Convert(syntaxNode.Right, syntaxNodeConverterContext, scope);
			var expression = Expression.MakeBinary(BinaryExpressionMap.ExpressionTypeMap[syntaxNode.Operator], left.Expression!, Expression.Convert(right.Expression!, left.Expression!.Type));
			return new ConverterResult(expression);
		}
	}
}
