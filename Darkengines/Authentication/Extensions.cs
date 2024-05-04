using Darkengines.Authentication.Entities;
using Darkengines.Authentication.Jwt;
using Darkengines.Data;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Authentication {
	public static class Extensions {
		public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, Action<AuthenticationOptions> configureOptions) {
			return serviceCollection.Configure(configureOptions)
			.AddSingleton<JwtAuthenticationConfiguration>()
			.AddScoped<Authentication>();
		}

		public static ModelBuilder ConfigureAuthentication(this ModelBuilder modelBuilder) {
			var userPasswordResetRequestEntityTypeBuilder = modelBuilder.Entity<UserPasswordResetRequest>();
			userPasswordResetRequestEntityTypeBuilder.AsMonitored();
			userPasswordResetRequestEntityTypeBuilder.HasKey(userPasswordResetRequest => new {
				userPasswordResetRequest.UserId,
				userPasswordResetRequest.Guid
			});
			userPasswordResetRequestEntityTypeBuilder.HasOne(userPasswordResetRequest => userPasswordResetRequest.User)
			.WithMany(user => user.UserPasswordResetRequests)
			.HasForeignKey(userPasswordResetRequest => userPasswordResetRequest.UserId);
			userPasswordResetRequestEntityTypeBuilder.HasIndex(userPasswordResetRequest => userPasswordResetRequest.Guid);

			return modelBuilder;
		}
	}
}
