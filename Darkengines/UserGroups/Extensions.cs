using Darkengines.Applications.Rules;
using Darkengines.UserGroups.Entities;
using Darkengines.UserGroups.Rules;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Darkengines.UserGroups {
	public static class Extensions {
		public static IServiceCollection AddUserGroups(this IServiceCollection serviceCollection) {
			return serviceCollection
				.AddSingleton<IRuleMap, UserGroupRule>()
				.AddSingleton<IRuleMap, UserUserGroupRule>();
		}
		public static ModelBuilder ConfigureUserGroups(this ModelBuilder modelBuilder) {
			var userGroupEntityTypeBuilder = modelBuilder.Entity<UserGroup>();
			userGroupEntityTypeBuilder.HasKey(user => user.Id);
			userGroupEntityTypeBuilder.Property(user => user.DisplayName).HasMaxLength(256).IsRequired();
			userGroupEntityTypeBuilder.Property(user => user.ExternalId).HasMaxLength(256).IsRequired();
			userGroupEntityTypeBuilder.HasIndex(userGroup => userGroup.ExternalId).HasFilter($"[{nameof(UserGroup.ExternalId)}] IS NOT NULL");
			userGroupEntityTypeBuilder.HasMany(userGroup => userGroup.UserUserGroups).WithOne(userUserGroup => userUserGroup.UserGroup).HasForeignKey(userUserGroup => userUserGroup.UserGroupId);

			var userUserGroupEntityTypeBuilder = modelBuilder.Entity<UserUserGroup>();
			userUserGroupEntityTypeBuilder.HasKey(userUserGroup => new { userUserGroup.UserId, userUserGroup.UserGroupId });
			userUserGroupEntityTypeBuilder.HasOne(userUserGroup => userUserGroup.User).WithMany(user => user.UserUserGroups).HasForeignKey(userUserGroup => userUserGroup.UserId);
			userUserGroupEntityTypeBuilder.HasOne(userUserGroup => userUserGroup.UserGroup).WithMany(userGroup => userGroup.UserUserGroups).HasForeignKey(userUserGroup => userUserGroup.UserGroupId);

			return modelBuilder;
		}
	}
}
