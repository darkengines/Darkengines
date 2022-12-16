using Darkengines.Expressions.Application.Applications.Rules;
using Darkengines.Expressions.Application.UserGroups.Entities;
using Darkengines.Expressions.Application.Users.Entities;
using Darkengines.Expressions.Security;
using Darkengines.Expressions.userGroup.userGroups.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.UserGroups {
	public static class Extensions {
		public static IServiceCollection AddUserGroups(this IServiceCollection serviceCollection) {
			return serviceCollection.AddSingleton<IRuleMap, UserGroupRule>().AddSingleton<IRuleMap, UserUserGroupRule>();
		}
		public static ModelBuilder ConfigureUserGroups(this ModelBuilder modelBuilder) {
			var userGroupEntityTypeBuilder = modelBuilder.Entity<UserGroup>();
			userGroupEntityTypeBuilder.HasKey(user => user.Id);
			userGroupEntityTypeBuilder.Property(user => user.DisplayName).HasMaxLength(256).IsRequired();
			userGroupEntityTypeBuilder.HasMany(userGroup => userGroup.UserUserGroups).WithOne(userUserGroup => userUserGroup.UserGroup).HasForeignKey(userUserGroup => userUserGroup.UserGroupId);

			var userUserGroupEntityTypeBuilder = modelBuilder.Entity<UserUserGroup>();
			userUserGroupEntityTypeBuilder.HasKey(userUserGroup => new { userUserGroup.UserId, userUserGroup.UserGroupId });
			userUserGroupEntityTypeBuilder.HasOne(userUserGroup => userUserGroup.User).WithMany(user => user.UserUserGroups).HasForeignKey(userUserGroup => userUserGroup.UserId);
			userUserGroupEntityTypeBuilder.HasOne(userUserGroup => userUserGroup.UserGroup).WithMany(userGroup => userGroup.UserUserGroups).HasForeignKey(userUserGroup => userUserGroup.UserGroupId);

			return modelBuilder;
		}
	}
}
