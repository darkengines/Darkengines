using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class ParameterSyntaxConverter : CSharpExpressionConverter<ParameterSyntax> {
		public override ConverterResult Convert(ParameterSyntax node, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var predefinedTypeSyntax = node.Type as PredefinedTypeSyntax;
			var parameterType = default(Type);
			if (predefinedTypeSyntax != null) {
				var typeName = predefinedTypeSyntax.Keyword.ValueText;
				parameterType = BuiltInDotNetTypeMap.GetClrType(typeName);
				if (parameterType == null) throw new InvalidOperationException($"Failed to resolve CLR type for {typeName}.");
			} else {
				parameterType = argument!.GenericArguments![0];
			}
			var expression = Expression.Parameter(parameterType, node.Identifier.ValueText);
			return new ConverterResult(expression);
		}
	}
}
