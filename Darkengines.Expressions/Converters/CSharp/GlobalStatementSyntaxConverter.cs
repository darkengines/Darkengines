using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class GlobalStatementSyntaxConverter : CSharpExpressionConverter<GlobalStatementSyntax> {
		public override ConverterResult Convert(GlobalStatementSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			return syntaxNodeConverterContext.Convert(Language, syntaxNode.Statement, scope);
		}
	}
}
