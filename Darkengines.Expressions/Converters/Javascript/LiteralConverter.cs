using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class LiteralConverter : JavascriptExpressionConverter<Esprima.Ast.Literal> {
		public override ConverterResult Convert(Esprima.Ast.Literal syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var expression = Expression.Constant(syntaxNode.Value);
			return new ConverterResult(expression);
		}
	}
}
