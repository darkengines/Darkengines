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
		Expression GetResolver(object key, Expression contextExpression, Expression instanceParameterExpression);
		Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceParameterExpression);
	}
}
