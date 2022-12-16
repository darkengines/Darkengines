using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class ParenthesizedLambdaExpressionSyntaxConverter : CSharpExpressionConverter<ParenthesizedLambdaExpressionSyntax> {
		protected LambdaExpressionConverter LambdaExpressionSyntaxConverter { get; }
		public ParenthesizedLambdaExpressionSyntaxConverter(LambdaExpressionConverter lambdaExpressionSyntaxConverter) {
			LambdaExpressionSyntaxConverter = lambdaExpressionSyntaxConverter;
		}
		public override ConverterResult Convert(ParenthesizedLambdaExpressionSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			return LambdaExpressionSyntaxConverter.Convert(Language, syntaxNode.ParameterList.Parameters.ToArray(), syntaxNode.ExpressionBody!, syntaxNodeConverterContext, scope, argument!);
		}
		public override bool IsGenericType => true;
		public override Type GetGenericType(ParenthesizedLambdaExpressionSyntax syntaxNode, ConversionArgument argument) {
			return LambdaExpressionSyntaxConverter.GetGenericType(syntaxNode.ParameterList.Parameters.ToArray(), syntaxNode.ExpressionBody, argument);
		}
	}
}
