using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class ArrowFunctionExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.ArrowFunctionExpression> {
		public Dictionary<int, Type> FuncTypes = typeof(Func<>).Assembly.GetTypes().Where(type => type.Name.StartsWith("Func`")).ToArray().ToDictionary(type => type.GetGenericArguments().Length);
		protected LambdaExpressionConverter LambdaExpressionConverter { get; }
		public ArrowFunctionExpressionConverter(LambdaExpressionConverter lambdaExpressionSyntaxConverter) {
			LambdaExpressionConverter = lambdaExpressionSyntaxConverter;
		}
		public override ConverterResult Convert(Esprima.Ast.ArrowFunctionExpression syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var parameterCount = syntaxNode.Params.Count;
			var identifiers = default(IDictionary<string, Expression>);
			if (argument?.GenericArguments == null) {
				identifiers = syntaxNode.Params.ToDictionary(parameter => ((Esprima.Ast.Identifier)parameter).Name!, parameter => {
					var result = syntaxNodeConverterContext.Convert(Language, parameter, scope);
					return result.Expression!;
				});
			} else {
				identifiers = syntaxNode.Params.Zip(argument.GenericArguments).ToDictionary(tuple => ((Esprima.Ast.Identifier)tuple.First).Name!, tuple => {
					var result = Expression.Parameter(tuple.Second, ((Esprima.Ast.Identifier)tuple.First).Name!);
					return (Expression)result;
				});
			}
			var parameterExpressions = identifiers.Values.Cast<ParameterExpression>();
			var scopedContext = new ConverterScope(scope) { Identifiers = identifiers };
			var bodyResult = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Body).Convert(syntaxNode.Body, syntaxNodeConverterContext, scopedContext);
			var bodyExpression = bodyResult.Expression!;

			var genericFunctionType = FuncTypes[parameterCount + 1];
			var genericArguments = identifiers.Values.Select(parameter => parameter.Type).Append(bodyExpression.Type).ToArray();
			var functionType = genericFunctionType.MakeGenericType(genericArguments);

			var expression = Expression.Lambda(functionType, bodyExpression, parameterExpressions);
			return new ConverterResult(expression);
		}
		public override Type GetGenericType(Esprima.Ast.ArrowFunctionExpression syntaxNode, ConversionArgument argument) {
			return argument?.ExpectedType ?? FuncTypes[syntaxNode.Params.Count + 1];
		}
		public override bool IsGenericType => true;
	}
}
