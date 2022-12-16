using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public class NonQueryExpressionVisitor : ExpressionVisitor {
		protected object Evaluate(Expression expression) {
			return Expression.Lambda<Func<object>>(expression).Compile()();
		}
		public NonQueryExpressionVisitor() {

		}
		[return: NotNullIfNotNull("node")]
		public override Expression? Visit(Expression? node) {
			if (node is NonQueryExpression) return node;
			if (node != null && typeof(IQueryable).IsAssignableFrom(node.Type)) {
				return node;
			}
			node = base.Visit(node);
			return node;
		}
		protected override Expression VisitConstant(ConstantExpression node) {
			return new NonQueryExpression(base.VisitConstant(node));
		}
		protected override Expression VisitMember(MemberExpression node) {
			var expression = Visit(node.Expression);
			if (expression is NonQueryExpression booleanExpression) return new NonQueryExpression(node.Update(booleanExpression.Expression));
			return node;
		}
		protected override Expression VisitUnary(UnaryExpression node) {
			var operand = Visit(node.Operand);
			if (operand is NonQueryExpression booleanExpression) return new NonQueryExpression(node.Update(booleanExpression.Expression));
			return base.VisitUnary(node);
		}
		protected override Expression VisitConditional(ConditionalExpression node) {
			var test = Visit(node.Test);
			var ifTrue = Visit(node.IfTrue);
			var ifFalse = Visit(node.IfFalse);
			var testBooleanExpression = test as NonQueryExpression;
			var ifTrueBooleanExpression = ifTrue as NonQueryExpression;
			var ifFalseBooleanExpression = ifFalse as NonQueryExpression;

			if (testBooleanExpression != null && ifTrueBooleanExpression != null && ifFalseBooleanExpression != null) {
				return new NonQueryExpression(node.Update(testBooleanExpression.Expression, ifTrueBooleanExpression.Expression, ifFalseBooleanExpression.Expression));
			} else if (testBooleanExpression != null) {
				var result = (bool)Evaluate(testBooleanExpression.Expression);

				if (result) {
					if (ifTrueBooleanExpression != null) {
						return ifTrueBooleanExpression;
					} else {
						return ifTrue;
					}
				} else {
					if (ifFalseBooleanExpression != null) {
						return ifFalseBooleanExpression;
					} else {
						return ifFalse;
					}
				}
			} else {
				return node;
			}
		}
		protected override Expression VisitBinary(BinaryExpression node) {
			var left = Visit(node.Left);
			var right = Visit(node.Right);
			var leftBooleanExpression = left as NonQueryExpression;
			var rightBooleanExpression = right as NonQueryExpression;
			if (leftBooleanExpression != null && rightBooleanExpression != null) {
				return new NonQueryExpression(node.Update(leftBooleanExpression.Expression, node.Conversion, rightBooleanExpression.Expression));
			}
			switch (node.NodeType) {
				case ExpressionType.OrElse: {
						if (leftBooleanExpression != null) {
							var result = (bool)Evaluate(leftBooleanExpression.Expression);
							if (!result) return right;
							return Expression.Constant(result);
						} else if (rightBooleanExpression != null) {
							var result = (bool)Evaluate(rightBooleanExpression.Expression);
							if (!result) return left;
							return Expression.Constant(result);
						} else {
							return node;
						}
					}
				case ExpressionType.AndAlso: {
						if (leftBooleanExpression != null) {
							var result = (bool)Evaluate(leftBooleanExpression.Expression);
							if (!result) Expression.Constant(result);
							return right;
						} else if (rightBooleanExpression != null) {
							var result = (bool)Evaluate(rightBooleanExpression.Expression);
							if (!result) return Expression.Constant(result);
							return left;
						} else {
							return node;
						}
					}
				default: {
						return base.VisitBinary(node);
					}
			}

		}
	}
}
