using Darkengines.Authentication.Entities;
using Darkengines.UserGroups.Entities;
using Darkengines.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Darkengines.Data {
	public class ApplicationDbContext : DbContext {
		public DbSet<User> Users { get; set; }
		public DbSet<UserEmailAddress> UserEmailAddresses { get; set; }
		public DbSet<UserPasswordResetRequest> UserPasswordResetRequests { get; set; }
		public DbSet<UserUserGroup> UserUserGroups { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }

		public ApplicationDbContext() : base() { }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
		}
	}
}
