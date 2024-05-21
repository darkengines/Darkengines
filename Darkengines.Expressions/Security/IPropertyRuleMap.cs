using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public interface IPropertyRuleMap {
		PropertyInfo PropertyInfo { get; }
		bool TryGetResolver(object key, Expression contextExpression, Expression instanceParameterExpression, out Expression? resolver);
		bool TryGetOperationResolver(Operation operation, Expression contextExpression, Expression instanceParameterExpression, out Expression? resolver);
		Expression GetResolver(object key, Expression contextExpression, Expression instanceParameterExpression);
		Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceParameterExpression);
	}
}
