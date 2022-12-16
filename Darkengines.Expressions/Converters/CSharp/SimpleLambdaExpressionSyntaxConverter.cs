using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class SimpleLambdaExpressionSyntaxConverter : CSharpExpressionConverter<SimpleLambdaExpressionSyntax> {
		protected LambdaExpressionConverter LambdaExpressionSyntaxConverter { get; }
		public SimpleLambdaExpressionSyntaxConverter(LambdaExpressionConverter lambdaExpressionSyntaxConverter) {
			LambdaExpressionSyntaxConverter = lambdaExpressionSyntaxConverter;
		}
		public override ConverterResult Convert(SimpleLambdaExpressionSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			return LambdaExpressionSyntaxConverter.Convert(Language, new[] { syntaxNode.Parameter }, syntaxNode.ExpressionBody!, syntaxNodeConverterContext, scope, new ConversionArgument() { GenericArguments = argument?.GenericArguments });
		}
		public override bool IsGenericType => true;
		public override Type GetGenericType(SimpleLambdaExpressionSyntax syntaxNode, ConversionArgument argument) {
			return LambdaExpressionSyntaxConverter.GetGenericType(new[] { syntaxNode.Parameter }, syntaxNode.ExpressionBody, argument);
		}
	}
}
