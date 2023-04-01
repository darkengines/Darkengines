using Darkengines.Data;
using Darkengines.Applications;
using Darkengines.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Darkengines.Data {
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
