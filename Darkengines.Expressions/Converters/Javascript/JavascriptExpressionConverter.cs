using Microsoft.CodeAnalysis;

namespace Darkengines.Expressions.Converters.Javascript {
	public abstract class JavascriptExpressionConverter<TJavascriptExpression> : Converter<TJavascriptExpression> where TJavascriptExpression : Esprima.Ast.Expression {
		public override string Language => "Javascript";
	}
}