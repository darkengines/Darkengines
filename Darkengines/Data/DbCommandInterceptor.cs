using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Data {
	public class DarkenginesDbCommandInterceptor: DbCommandInterceptor {
		public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result) {
			return base.ReaderExecuting(command, eventData, result);
		}
		public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default) {
			return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
		}
		public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result) {
			return base.ReaderExecuted(command, eventData, result);
		}
	}
}
