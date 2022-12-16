using System.Linq.Expressions;

namespace Darkengines.Expressions.Converters {
	public class ConverterResult {
		public Expression? Expression { get; }
		public bool IsAsynchronous { get; }
		public ConverterResult(Expression? expression, bool isAsynchronous = false) {
			Expression = expression;
			IsAsynchronous = isAsynchronous;
		}
	}
}