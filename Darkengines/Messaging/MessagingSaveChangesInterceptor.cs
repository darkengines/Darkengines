using Darkengines.Data;
using Darkengines.Applications;
using Darkengines.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Darkengines.Expressions.Security;

namespace Darkengines.Data {
	public class MessagingSaveChangesInterceptor : SaveChangesInterceptor {
		protected ICollection<IRuleMap> RuleMaps { get; }
		protected IApplicationContext ApplicationContext { get; }
		public MessagingSaveChangesInterceptor(
			IApplicationContext applicationContext,
			ICollection<IRuleMap> ruleMaps
		) {
			ApplicationContext = applicationContext;
			RuleMaps = ruleMaps;
		}
		public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			var ruleMaps = RuleMaps.ToDictionary(ruleMap => ruleMap.Type);
			foreach (var entry in eventData.Context.ChangeTracker.Entries()) {
				if (ruleMaps.TryGetValue(entry.Metadata.ClrType, out var ruleMap)) {
					ruleMap.GetOperationResolver(Operation.Read,)
				}
			}
			return await base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
