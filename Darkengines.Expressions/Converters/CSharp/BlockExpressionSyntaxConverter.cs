using Esprima;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class BlockExpressionSyntaxConverter : CSharpExpressionConverter<BlockSyntax> {
		public BlockExpressionSyntaxConverter() {
		}
		public override ConverterResult Convert(BlockSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			throw new NotImplementedException();
		}
	}
}
