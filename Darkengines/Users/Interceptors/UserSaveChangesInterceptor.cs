using Comeet.Core.Data;
using Darkengines.Applications;
using Darkengines.Authentication;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Users.Interceptors {
	public class UserSaveChangesInterceptor : SaveChangesInterceptor {
		protected IApplicationContext ApplicationContext { get; }
		protected IEnumerable<IRuleMap> Rulemaps { get; }
		public UserSaveChangesInterceptor(IApplicationContext applicationContext, IEnumerable<IRuleMap> rulemaps) {
			ApplicationContext = applicationContext;
			Rulemaps = rulemaps;
		}
		//public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
		//	foreach (var userEmailAddressEntry in eventData.Context.ChangeTracker.Entries<UserEmailAddress>().Where(entry => entry.State == EntityState.Added)) {
		//		var ruleMap = Rulemaps.FirstOrDefault(rulemap => rulemap.Type == typeof(UserEmailAddress));
		//		var resolver = ruleMap.GetOperationResolver(Operation.Read, ApplicationContext, Expression.Constant(userEmailAddressEntry.Entity));
		//		var lambda = Expression.Lambda<Func<bool>>(resolver);
		//		var function = lambda.Compile();
		//		var hasPermission = function();
		//		if (!hasPermission) userEmailAddressEntry.Entity.Guid = null;
		//	}
		//	return await base.SavingChangesAsync(eventData, result, cancellationToken);
		//}

		public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default) {
			foreach (var userEmailAddressEntry in eventData.Context.ChangeTracker.Entries<UserEmailAddress>().Where(entry => entry.State == EntityState.Added)) {
				Console.WriteLine($"SEND {userEmailAddressEntry.Entity.EmailAddress} {userEmailAddressEntry.Entity.Guid}");
			}
			return base.SavedChangesAsync(eventData, result, cancellationToken);
		}
	}
}
