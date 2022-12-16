using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions {
	public static class DbContextExtension {
		public static IQueryable GetQuery<TDbContext>(this TDbContext dbContext, Type type) where TDbContext : DbContext {
			var queryMethodInfo = ExpressionHelper.ExtractMethodInfo<TDbContext, Func<IQueryable<object>>>(dbContext => dbContext.Set<object>).GetGenericMethodDefinition();
			return (IQueryable)queryMethodInfo.MakeGenericMethod(type).Invoke(dbContext, new object[0]);
		}
	}
}
