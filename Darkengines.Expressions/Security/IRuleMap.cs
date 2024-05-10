using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public interface IRuleMap {
		Type Type { get; }
		Expression GetResolver(object key, Expression contextExpression, Expression instanceParameterExpression);
		Expression GetOperationResolver(Operation operation, Expression contextExpression, Expression instanceParameterExpression);
		public Expression? GetPropertyResolver(PropertyInfo propertyInfo, object key, Expression contextExpression, Expression instanceExpression);
		public Expression? GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, Expression contextExpression, Expression instanceExpression);
	}
}
