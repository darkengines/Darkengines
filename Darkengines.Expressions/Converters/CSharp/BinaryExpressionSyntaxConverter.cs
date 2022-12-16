using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class BinaryExpressionSyntaxConverter : CSharpExpressionConverter<BinaryExpressionSyntax> {
		public override ConverterResult Convert(BinaryExpressionSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var left = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Left).Convert(syntaxNode.Left, syntaxNodeConverterContext, scope);
			var right = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Right).Convert(syntaxNode.Right, syntaxNodeConverterContext, scope);
			var expression = Expression.MakeBinary(BinaryExpressionMap.ExpressionTypeMap[syntaxNode.Kind()], left.Expression!, Expression.Convert(right.Expression!, left.Expression!.Type));
			return new ConverterResult(expression);
		}
	}
}
