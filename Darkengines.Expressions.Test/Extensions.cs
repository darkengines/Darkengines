﻿using Darkengines.Core;
using Darkengines.Core.Applications;
using Darkengines.Core.Authentication;
using Darkengines.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darkengines.Core.Test {
	public static partial class Extensions {
		public static IServiceCollection AddTestApplicationContext(this IServiceCollection serviceCollection) {

			serviceCollection.AddSingleton<ApplicationInitializationContext>()
			.AddSingleton<IIdentityProvider>(serviceProvider => serviceProvider.GetRequiredService<ApplicationInitializationContext>());
			return serviceCollection.AddSingleton<ApplicationInitializer, ApplicationInitializer>()
				.AddSingleton<IApplicationContext, ApplicationContext>()
				.AddSingleton<TestApplicationContext>()
				.AddDarkengines()
				.AddDbContext<ApplicationDbContext>((serviceProvider, options) => {
					var model = serviceProvider.GetRequiredService<IModel>();
					var interceptors = serviceProvider.GetRequiredService<IEnumerable<IInterceptor>>();
					var configuration = serviceProvider.GetRequiredService<IConfiguration>();
					options.AddInterceptors(interceptors);
					options.UseInternalServiceProvider(serviceProvider);
					options.UseSqlServer(configuration.GetConnectionString("default"), sqlServerOptions => {
						sqlServerOptions.CommandTimeout(60);
						sqlServerOptions.UseNetTopologySuite();
					});
					options.UseModel(model);
					options.EnableSensitiveDataLogging();
				});
		}
	}
}
