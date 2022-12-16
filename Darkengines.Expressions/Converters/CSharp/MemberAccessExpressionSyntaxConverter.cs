using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.CSharp {
	public class MemberAccessExpressionSyntaxConverter : CSharpExpressionConverter<MemberAccessExpressionSyntax> {
		public override ConverterResult Convert(MemberAccessExpressionSyntax syntaxNode, ConversionContext syntaxNodeConverterContext, ConverterScope scope, ConversionArgument? argument) {
			var objectConvertionResult = syntaxNodeConverterContext.GetSyntaxNodeConverter(Language, syntaxNode.Expression).Convert(syntaxNode.Expression, syntaxNodeConverterContext, scope);
			var type = default(Type);
			if (objectConvertionResult.Expression != null) {
				type = objectConvertionResult.Expression.Type;
			} else {
				var typeName = syntaxNode.Expression.GetText().ToString();
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
				var memberInfo = type.GetMember(syntaxNode.Name.Identifier.Text);
				if (memberInfo != null) {
					var expression = Expression.MakeMemberAccess(objectConvertionResult.Expression, memberInfo[0]);
					return new ConverterResult(expression);
				} else {
					throw new MissingMemberException($"Unable to find member {syntaxNode.Name.Identifier.Text} on type {type.Name}.");
				}
			}
			return new ConverterResult(null);
		}
	}
}
