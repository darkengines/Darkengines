using Darkengines.Expressions.Mutation;
using Darkengines.Users.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Users.Interceptors {
	public class UserMutationInterceptor : IMutationInterceptor {
		public void PopulatingEntity(JObject jObject, EntityEntry entityEntry) {
			if (entityEntry.Entity is User user) {
				if (jObject.TryGetValue("password", StringComparison.InvariantCultureIgnoreCase, out var passwordToken)) {
					var password = passwordToken.Value<string>();
					user.HashedPassword = password.ToSHA256();
				}
			}
		}
	}
}
