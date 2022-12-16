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
		Expression GetResolver(object key, object context, Expression instanceParameterExpression);
		Expression GetOperationResolver(Operation operation, object context, Expression instanceParameterExpression);
		public Expression? GetPropertyResolver(PropertyInfo propertyInfo, object key, object context, Expression instanceExpression);
		public Expression? GetPropertyOperationResolver(PropertyInfo propertyInfo, Operation operation, object context, Expression instanceExpression);
	}
}
