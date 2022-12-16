using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class IncompleteMemberSyntaxConverter : CSharpExpressionConverter<IncompleteMemberSyntax> {
		public override ConverterResult Convert(IncompleteMemberSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var child = syntaxNode.ChildNodes().First();
			var result = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, child).Convert(child, syntaxNodeConverterContext, scope);
			var expression = result.Expression;
			return new ConverterResult(expression);
		}
	}
}
