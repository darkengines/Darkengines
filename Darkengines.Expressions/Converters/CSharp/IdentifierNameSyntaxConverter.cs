using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class IdentifierNameSyntaxConverter : CSharpExpressionConverter<IdentifierNameSyntax> {
		public override ConverterResult Convert(IdentifierNameSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var expression = scope.ResolveIdentifier(syntaxNode.Identifier.Text);
			return new ConverterResult(expression);
		}
	}
}
