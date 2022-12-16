using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public class NonQueryExpression : Expression {
		public Expression Expression { get; }
		public NonQueryExpression(Expression expression) {
			Expression = expression;
		}
		public override Type Type => Expression.Type;
		public override ExpressionType NodeType => ExpressionType.Extension;
		protected override Expression Accept(ExpressionVisitor visitor) {
			return visitor.Visit(Expression);
		}
		protected override Expression VisitChildren(ExpressionVisitor visitor) {
			return visitor.Visit(Expression.Reduce());
		}
		protected Delegate? EvaluateFunction = default(Delegate);
		public TResult Evaluate<TResult>() {
			if (EvaluateFunction == null) {
				EvaluateFunction = Lambda<Func<TResult>>(this).Compile();
			}
			return ((Func<TResult>)EvaluateFunction)();
		}
	}
}
