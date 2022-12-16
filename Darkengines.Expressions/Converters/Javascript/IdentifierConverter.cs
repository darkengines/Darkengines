using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class IdentifierConverter : JavascriptExpressionConverter<Esprima.Ast.Identifier> {
		public override ConverterResult Convert(Esprima.Ast.Identifier node, ConversionContext converterContext, ConverterScope scope, ConversionArgument? argument) {
			var expression = scope.ResolveIdentifier(node.Name!);
			return new ConverterResult(expression);
		}
	}
}
