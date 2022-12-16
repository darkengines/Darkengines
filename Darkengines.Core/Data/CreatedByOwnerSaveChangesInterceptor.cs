using Comeet.Core.Data;
using Darkengines.Core.Applications;
using Darkengines.Core.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Darkengines.Core.Data {
	public class CreatedByOwnerSaveChangesInterceptor : SaveChangesInterceptor {
		protected IApplicationContext ApplicationContext { get; }
		public CreatedByOwnerSaveChangesInterceptor(IApplicationContext applicationContext) {
			ApplicationContext = applicationContext;
		}
		public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			foreach (var createdByOwner in eventData.Context.ChangeTracker.Entries<ICreatedByOwner>()) {
				createdByOwner.Entity.CreatedById = ApplicationContext.CurrentUser?.Id;
			}
			return await base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
