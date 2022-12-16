using Microsoft.CodeAnalysis;

namespace Darkengines.Expressions.Converters.CSharp {
	public abstract class CSharpExpressionConverter<TSyntaxNode> : Converter<TSyntaxNode> where TSyntaxNode : SyntaxNode {
		public override string Language => "CSharp";
	}
}