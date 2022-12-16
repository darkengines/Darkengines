﻿using Darkengines.Core.Applications.Rules;
using Darkengines.Core.UserGroups.Entities;
using Darkengines.Core.UserGroups.Rules;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Darkengines.Core.UserGroups {
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
