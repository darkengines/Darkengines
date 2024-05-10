using Darkengines.Messaging.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Messaging {
	public static class Extensions {
		public static ModelBuilder AddModels(this ModelBuilder modelBuilder) {
			var modelEntityBuilder = modelBuilder.Entity<Client>();
			modelEntityBuilder.HasKey(model => model.ConnectionId);
			modelEntityBuilder.HasIndex(model => new { model.ConnectionId, model.UserId });
			modelEntityBuilder.Property(model => model.Host).HasMaxLength(256).IsRequired();
			modelEntityBuilder.Property(model => model.UserId).IsRequired();
			modelEntityBuilder.Property(model => model.ConnectionId).IsRequired();
			modelEntityBuilder.HasOne(model => model.User).WithMany(user => user.Clients).HasForeignKey(client => client.UserId);

			return modelBuilder;
		}
	}
}
