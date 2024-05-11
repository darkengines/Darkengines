using Darkengines.Data;
using Darkengines.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Data {
	public static class Extensions {
		public static IServiceCollection AddData(this IServiceCollection serviceCollection) {
			return serviceCollection
				.AddSingleton<ApplicationDbContextFactory>()
				.AddScoped<Mutation>()
				.AddScoped<IInterceptor, ModificationByOwnerSaveChangesInterceptor>()
				.AddScoped<IInterceptor, CreationDateOwnerSaveChangesInterceptor>()
				.AddScoped<IInterceptor, ModificationDateOwnerSaveChangesInterceptor>()
				.AddScoped<IInterceptor, DarkenginesDbCommandInterceptor>();
		}
		public static EntityTypeBuilder<TMonitored> AsMonitored<TMonitored>(this EntityTypeBuilder<TMonitored> entityTypeBuilder) where TMonitored : class, IMonitored {
			entityTypeBuilder.HasOne(monitored => monitored.CreatedBy).WithMany().HasForeignKey(monitored => monitored.CreatedById).OnDelete(DeleteBehavior.NoAction);
			entityTypeBuilder.HasOne(monitored => monitored.ModifiedBy).WithMany().HasForeignKey(monitored => monitored.ModifiedById).OnDelete(DeleteBehavior.NoAction);
			return entityTypeBuilder;
		}
		public static EntityTypeBuilder<TEntity> AsActiveStateOwner<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, IActiveStateOwner {
			entityTypeBuilder.Property(activeStateOwner => activeStateOwner.IsActive).HasDefaultValue(true);
			entityTypeBuilder.HasOne(monitored => monitored.DeactivatedByUser).WithMany().HasForeignKey(monitored => monitored.DeactivatedByUserId).OnDelete(DeleteBehavior.NoAction);
			return entityTypeBuilder;
		}
	}
}
