using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public class StaticMemberExpressionConverter : JavascriptExpressionConverter<Esprima.Ast.StaticMemberExpression> {
		public override ConverterResult Convert(Esprima.Ast.StaticMemberExpression syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var objectConvertionResult = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Object).Convert(syntaxNode.Object, syntaxNodeConverterContext, scope);
			var type = default(Type);
			if (objectConvertionResult.Expression != null) {
				type = objectConvertionResult.Expression.Type;
			} else {
				var typeName = syntaxNode.Object.ToString();
				var typeArgumentSyntaxIndex = typeName.LastIndexOf('<');
				if (typeArgumentSyntaxIndex >= 0) {
					var nakedTypeName = typeName.Substring(0, typeArgumentSyntaxIndex);
					var typeArgumentsText = typeName.Substring(typeArgumentSyntaxIndex);
					var typeArguments = typeArgumentsText.Substring(1, typeArgumentsText.Length - 2).Split(',').Select(typeArgument => {
						var type = BuiltInDotNetTypeMap.GetClrType(typeArgument.Trim());
						if (type == null) throw new InvalidOperationException($"Failed to resolve CLR type for {typeName}.");
						return type;
					}).ToArray();
					var genericTypeName = $@"{nakedTypeName}`{typeArguments.Length}";
					type = Type.GetType(genericTypeName)?.MakeGenericType(typeArguments);
				} else {
					type = Type.GetType(typeName);
				}
			}
			if (type != null) {
				if (syntaxNode.Property is Esprima.Ast.Identifier javascriptMemberIdentifier) {
					var memberInfo = type.GetMember(javascriptMemberIdentifier.Name!);
					if (memberInfo != null) {
						var expression = Expression.MakeMemberAccess(objectConvertionResult.Expression, memberInfo[0]);
						return new ConverterResult(expression);
					} else {
						throw new MissingMemberException($"Unable to find member {javascriptMemberIdentifier.Name} on type {type.Name}.");
					}
				} else {
					throw new MissingMemberException($"Unable to resolve member {syntaxNode.Property} on type {type.Name}.");
				}
			}
			return new ConverterResult(null);
		}
	}
}
