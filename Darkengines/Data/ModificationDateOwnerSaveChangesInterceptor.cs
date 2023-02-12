using Comeet.Core.Data;
using Darkengines.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Data {
	public class ModificationDateOwnerSaveChangesInterceptor : SaveChangesInterceptor {
		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
			foreach (var createdByOwner in eventData.Context.ChangeTracker.Entries<IModificationDateOwner>()) {
				createdByOwner.Entity.ModifiedOn = DateTimeOffset.UtcNow;
			}
			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
