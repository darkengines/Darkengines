using Darkengines.Core.Authentication.Entities;
using Darkengines.Core.UserGroups.Entities;
using Darkengines.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Darkengines.Core.Data {
	public class ApplicationDbContext : DbContext {
		public DbSet<User> Users { get; set; }
		public DbSet<UserEmailAddress> UserEmailAddresses { get; set; }
		public DbSet<UserPasswordResetRequest> UserPasswordResetRequests { get; set; }
		public DbSet<UserUserGroup> UserUserGroups { get; set; }
		public ApplicationDbContext() : base() { }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
		}
	}
}
