using Comeet.Core.Data;
using Darkengines.Core.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Data {
	public class CreationDateOwnerSaveChangesInterceptor : SaveChangesInterceptor {
		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			foreach (var createdByOwner in eventData.Context.ChangeTracker.Entries<ICreationDateOwner>()) {
				createdByOwner.Entity.CreatedOn = DateTimeOffset.UtcNow;
			}
			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
