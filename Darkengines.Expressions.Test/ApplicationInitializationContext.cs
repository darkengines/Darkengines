using Darkengines.Data;
using Darkengines.Users.Entities;
using Darkengines.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Darkengines.Authentication;

namespace Darkengines.Test {
	public static partial class Extensions {
		public class ApplicationInitializationContext : IIdentityProvider {
			protected IConfiguration Configuration { get; }
			protected IModel Model { get; }
			public Task<IIdentity> Identity { get; }
			public Task<User> RootUser { get; }
			public ApplicationInitializationContext(IConfiguration configuration, IModel model) {
				Configuration = configuration;
				Model = model;

				var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
				optionBuilder.UseSqlServer(Configuration.GetConnectionString("default"));
				optionBuilder.UseModel(Model);
				optionBuilder.EnableSensitiveDataLogging();
				RootUser = CreateDbContextAsync().ContinueWith(async dbContextTask => {
					var rootUser = await CreateRootUser(dbContextTask.Result);
					await dbContextTask.Result.DisposeAsync();
					return rootUser;
				}).ContinueWith(task => (task.Result).Result);
			}
			protected async Task<ApplicationDbContext> CreateDbContextAsync() {
				var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
				optionBuilder.UseSqlServer(Configuration.GetConnectionString("default"));
				optionBuilder.UseModel(Model);
				optionBuilder.EnableSensitiveDataLogging();
				var dbContext = new ApplicationDbContext(optionBuilder.Options);
				await dbContext.Database.EnsureDeletedAsync();
				await dbContext.Database.MigrateAsync();

				return dbContext;
			}
			public async Task<User> CreateRootUser(ApplicationDbContext applicationDbContext) {
				var random = new Random();
				var passwordBytes = new byte[32];
				random.NextBytes(passwordBytes);

				using (var sha256 = SHA256.Create()) {
					var hashedPassword = sha256.ComputeHash(passwordBytes);
					var root = new User {
						HashedPassword = hashedPassword,
					};
					var emailAddress = "root@darkengines.com";
					root.UserEmailAddresses.Add(new UserEmailAddress {
						EmailAddress = emailAddress,
						HashedEmailAddress = Authentication.Authentication.ToLowerInvariantSHA256(emailAddress),
						IsVerified = true,
					});
					root.UserProfile = new UserProfile {
						DisplayName = "root",
						Firstname = "Root",
						Lastname = "Darkengines",
						Gender = Gender.None,
					};
					root.UserSettings = new UserSettings();

					await applicationDbContext.Users.AddAsync(root);
					await applicationDbContext.SaveChangesAsync();

					return root;
				}
			}

			public IIdentity GetIdentity() {
				Identity.Wait();
				return Identity.Result;
			}

			public async Task<IIdentity> GetIdentityAsync() {
				return await Identity;
			}
		}
	}
}
