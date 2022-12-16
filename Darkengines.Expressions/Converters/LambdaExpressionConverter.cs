using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public class LambdaExpressionConverter {
		public Dictionary<int, Type> FuncTypes = typeof(Func<>).Assembly.GetTypes().Where(type => type.Name.StartsWith("Func`")).ToArray().ToDictionary(type => type.GetGenericArguments().Length);
		public ConverterResult Convert(string language, ParameterSyntax[] parameterNodes, ExpressionSyntax bodyNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument conversionArgument) {
			var parameterCount = parameterNodes.Length;
			var identifiers = default(IDictionary<string, Expression>);
			if (conversionArgument?.GenericArguments == null) {
				identifiers = parameterNodes.ToDictionary(parameter => parameter.Identifier.ValueText, parameter => {
					var result = syntaxNodeConverterContext.Convert(language, parameter, scope);
					return result.Expression!;
				});
			} else {
				identifiers = parameterNodes.Zip(conversionArgument.GenericArguments).ToDictionary(tuple => tuple.First.Identifier.ValueText, tuple => {
					var result = syntaxNodeConverterContext.GetSyntaxNodeConverter(language, tuple.First).Convert(
						tuple.First, 
						syntaxNodeConverterContext, 
						scope, 
						new ConversionArgument() { 
							GenericArguments = new[] { tuple.Second } 
						}
					);
					return result.Expression!;
				});
			}
			var parameterExpressions = identifiers.Values.Cast<ParameterExpression>();
			var scopedContext = new ConverterScope(scope) { Identifiers = identifiers };
			var bodyResult = syntaxNodeConverterContext.GetSyntaxNodeConverter(language, bodyNode).Convert(bodyNode, syntaxNodeConverterContext, scope);
			var bodyExpression = bodyResult.Expression!;

			var genericFunctionType = FuncTypes[parameterCount + 1];
			var genericArguments = identifiers.Values.Select(parameter => parameter.Type).Append(bodyExpression.Type).ToArray();
			var functionType = genericFunctionType.MakeGenericType(genericArguments);
			var expression = Expression.Lambda(functionType, bodyExpression, parameterExpressions);
			return new ConverterResult(expression);
		}
		public Type GetGenericType(ParameterSyntax[] parameterNodes, ExpressionSyntax? bodyNode, ConversionArgument conversionArgument) {
			return FuncTypes[parameterNodes.Length + 1];
		}
	}
}
