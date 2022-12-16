using Darkengines.Expressions.Application.Users.Entities;
using Darkengines.Expressions.Application.Users.Rules;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Users {
	public static class Extensions {
		public static IServiceCollection AddUsers(this IServiceCollection serviceCollection) {
			return serviceCollection.AddSingleton<IRuleMap, UserRule>();
		}
		public static ModelBuilder ConfigureUsers(this ModelBuilder modelBuilder) {
			var userEntityTypeBuilder = modelBuilder.Entity<User>();
			userEntityTypeBuilder.HasKey(user => user.Id);
			userEntityTypeBuilder.Property(user => user.DisplayName).HasMaxLength(256).IsRequired();
			userEntityTypeBuilder.Property(user => user.Firstname).HasMaxLength(256).IsRequired();
			userEntityTypeBuilder.Property(user => user.Lastname).HasMaxLength(256).IsRequired();
			userEntityTypeBuilder.Property(user => user.EmailAddress).HasMaxLength(256).IsRequired();
			userEntityTypeBuilder.HasMany(user => user.UserUserGroups).WithOne(userUserGroup => userUserGroup.User).HasForeignKey(userUserGroup => userUserGroup.UserId);
			userEntityTypeBuilder.HasOne(user => user.Application).WithMany(application => application.Users).HasForeignKey(user => user.ApplicationId);
			return modelBuilder;
		}
	}
}
