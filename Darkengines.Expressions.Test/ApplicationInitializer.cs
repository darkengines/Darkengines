using Darkengines.Core.Applications.Entities;
using Darkengines.Core.Authentication;
using Darkengines.Core.Data;
using Darkengines.Core.Test;
using Darkengines.Core.UserGroups.Entities;
using Darkengines.Core.Users;
using Darkengines.Core.Users.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Test {
	public class ApplicationInitializer {
		protected ApplicationDbContext ApplicationDbContext { get; }
		public ApplicationInitializer(ApplicationDbContext applicationDbContext) {
			ApplicationDbContext = applicationDbContext;
		}
		public async Task Initialize() {
			await FillDatabase();
		}
		public async Task FillDatabase() {
			var random = new Random();
			var applications = Enumerable.Range(0, 16).Select(applicationIndex =>
				new Application {
					Name = $"Application{applicationIndex}",
					DisplayName = $"Application{applicationIndex}"
				}
			).ToArray();

			var users = Enumerable.Range(0, 100).Select(index => {
				var user = new User {
					HashedPassword = new byte[32],
					UserProfile = new UserProfile() {
						DisplayName = $"Slayer #{index}",
						Firstname = $"Guy #{index}",
						Lastname = "Doom",
					}
				};
				random.NextBytes(user.HashedPassword);
				var emailAddress = $"slayer{index}@uac.com";
				var hashedEmailAddress = Authentication.Authentication.ToLowerInvariantSHA256(emailAddress);
				user.UserEmailAddresses.Add(new UserEmailAddress { EmailAddress = emailAddress, HashedEmailAddress = hashedEmailAddress, User = user });
				return user;
			}).ToArray();

			var userGroups = Enumerable.Range(1, 10).Select(index => {
				var userGroup = new UserGroup() {
					DisplayName = $"Hell #{index}"
				};
				var userUserGroups = users.Select(user => new UserUserGroup() {
					User = user,
					UserGroup = userGroup
				});
				foreach (var userUserGroup in userUserGroups) userGroup.UserUserGroups.Add(userUserGroup);
				return userGroup;
			}).ToArray();

			await ApplicationDbContext.AddRangeAsync(applications);
			await ApplicationDbContext.AddRangeAsync(users);
			await ApplicationDbContext.AddRangeAsync(userGroups);
			await ApplicationDbContext.SaveChangesAsync();
		}
	}
}
