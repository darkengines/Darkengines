using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class LiteralExpressionSyntaxConverter : CSharpExpressionConverter<LiteralExpressionSyntax> {
		public override ConverterResult Convert(LiteralExpressionSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var expression = Expression.Constant(syntaxNode.Token.Value);
			return new ConverterResult(expression);
		}
	}
}
