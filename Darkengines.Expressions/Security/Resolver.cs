using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public delegate TResult Resolver<TItem, TContext, TResult>(TItem instance, TContext context);
	public delegate TResult PropertyResolver<TItem, TProperty, TContext, TResult>(TItem instance, TContext context);
}
