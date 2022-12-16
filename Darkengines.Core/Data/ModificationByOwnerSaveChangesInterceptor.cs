using Comeet.Core.Data;
using Darkengines.Core.Applications;
using Darkengines.Core.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Data {
	public class ModificationByOwnerSaveChangesInterceptor : SaveChangesInterceptor {
		protected IApplicationContext ApplicationContext { get; }
		public ModificationByOwnerSaveChangesInterceptor(IApplicationContext applicationContext) {
			ApplicationContext = applicationContext;
		}
		public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			foreach (var createdByOwner in eventData.Context.ChangeTracker.Entries<IModificationByOwner>()) {
				createdByOwner.Entity.ModifiedById = ApplicationContext.CurrentUser?.Id;
			}
			return await base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
