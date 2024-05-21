using Darkengines.Data;
using Darkengines.Messaging.Entities;
using Darkengines.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Messaging {
	public static class Extensions {
		public static IServiceCollection AddMessaging(this IServiceCollection services) {
			return services
				.AddSingleton<MessagingSystem>()
				.AddScoped<IInterceptor, MessagingSaveChangesInterceptor>();
		}
		public static IApplicationBuilder UseMessaging(this IApplicationBuilder applicationBuilder) {
			return applicationBuilder.UseMiddleware<MessagingMiddleware>();
		}
		public static ModelBuilder ConfigureMessaging(this ModelBuilder modelBuilder) {
			var modelEntityBuilder = modelBuilder.Entity<Client>();
			modelEntityBuilder.HasKey(model => model.ConnectionId);
			modelEntityBuilder.HasIndex(model => new { model.ConnectionId, model.UserId });
			modelEntityBuilder.Property(model => model.Host).HasMaxLength(256).IsRequired();
			modelEntityBuilder.Property(model => model.LastKeepAliveDateTime).IsRequired();
			modelEntityBuilder.Property(model => model.UserId).IsRequired();
			modelEntityBuilder.Property(model => model.ConnectionId).IsRequired();
			modelEntityBuilder.HasOne(model => model.User).WithMany(user => user.Clients).HasForeignKey(client => client.UserId);

			return modelBuilder;
		}
	}
}
