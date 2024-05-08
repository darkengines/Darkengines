using Darkengines.Data;
using Darkengines.Models;
using Darkengines.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Storage {
	public static class Extensions {
		public static IServiceCollection AddStorage(this IServiceCollection serviceCollection) {
			return serviceCollection;
		}
		public static ModelBuilder ConfigureStorage(this ModelBuilder modelBuilder) {
			var userEntityTypeBuilder = modelBuilder.Entity<Entities.File>();
			userEntityTypeBuilder.AsMonitored();
			userEntityTypeBuilder.HasKey(file => file.Id);
			userEntityTypeBuilder.Property(file => file.Uri).HasMaxLength(2048);
			userEntityTypeBuilder.Property(file => file.ContentType).HasMaxLength(256);
			userEntityTypeBuilder.Property(file => file.Length);

			return modelBuilder;
		}
	}
}
