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
		Expression GetResolver(object key, object context, Expression instanceParameterExpression);
		Expression GetOperationResolver(Operation operation, object context, Expression instanceParameterExpression);
	}
}
