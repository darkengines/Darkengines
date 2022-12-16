using Darkengines.Expressions.Application.Applications.Rules;
using Darkengines.Expressions.Application.Users.Entities;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Applications {
	public static class Extensions {
		public static IServiceCollection AddApplications(this IServiceCollection serviceCollection) {
			return serviceCollection.AddSingleton<IRuleMap, ApplicationRule>();
		}
		public static ModelBuilder ConfigureApplications(this ModelBuilder modelBuilder) {
			var applicationEntityTypeBuilder = modelBuilder.Entity<Entities.Application>();
			applicationEntityTypeBuilder.HasKey(application => application.Id);
			applicationEntityTypeBuilder.Property(application => application.DisplayName).HasMaxLength(256).IsRequired();
			applicationEntityTypeBuilder.Property(application => application.Name).HasMaxLength(256).IsRequired();
			applicationEntityTypeBuilder.HasMany(application => application.Users).WithOne(user => user.Application).HasForeignKey(user => user.ApplicationId);

			return modelBuilder;
		}
	}
}
