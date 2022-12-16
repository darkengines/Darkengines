using Darkengines.Core.Applications;
using Darkengines.Core.Authentication.Jwt;
using Darkengines.Core.Models;
using Darkengines.Core.Serialization;
using Darkengines.Core.UserGroups;
using Darkengines.Core.Users;
using Esprima.Ast;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkengines.Core.Data;
using Darkengines.Core.Authentication;
using Darkengines.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Darkengines.Core {
	public static class Extensions {
		public static IServiceCollection AddDarkenginesModel(this IServiceCollection serviceCollection, bool isDesignTime = false) {
			serviceCollection.AddSingleton((serviceProvider) => {
				var modelBuilder = CreateModelBuilder();
				var modelCustomizers = serviceProvider.GetServices<IModelCustomizer>();
				var modelRuntimeInitializer = serviceProvider.GetRequiredService<IModelRuntimeInitializer>();
				modelBuilder = modelBuilder.ConfigureUsers();
				modelBuilder = modelBuilder.ConfigureUserGroups();
				modelBuilder = modelBuilder.ConfigureAuthentication();
				modelBuilder = modelBuilder.ConfigureApplications();
				var model = modelBuilder.Model.FinalizeModel();
				model = modelRuntimeInitializer.Initialize(model, isDesignTime);
				return model;
			});
			return serviceCollection;
		}
		public static IServiceCollection AddDarkengines(this IServiceCollection serviceCollection) {
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			var configuration = configurationBuilder.Build();
			serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			serviceCollection.AddSingleton<IConfiguration>(configuration);
			serviceCollection.AddEntityFrameworkSqlServer();
			serviceCollection.AddEntityFrameworkSqlServerNetTopologySuite();
			serviceCollection.AddDarkenginesModel(false);
			serviceCollection.AddUsers();
			serviceCollection.AddUserGroups();
			serviceCollection.AddApplications();
			serviceCollection.AddModels();
			serviceCollection.AddData();
			serviceCollection.AddExpressions();
			serviceCollection.AddJsonSerializer();
			serviceCollection.AddJwtAuthentication(options => configuration.GetSection("Authentication.Jwt").Bind(options));

			return serviceCollection;
		}
		public static ModelBuilder CreateModelBuilder() {
			//using var serviceScope = CreateServiceScope();
			//using var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
			//return new ModelBuilder(ConventionSet.CreateConventionSet(context), context.GetService<ModelDependencies>());
			var modelBuilder = SqlServerConventionSetBuilder.CreateModelBuilder();
			return modelBuilder;
		}

		private static IServiceScope CreateServiceScope() {
			var serviceProvider = new ServiceCollection()
				.AddEntityFrameworkSqlServer()
				.AddEntityFrameworkSqlServerNetTopologySuite()
				.AddDbContext<DbContext>(
					(p, o) =>
						o.UseSqlServer("Server=.", sqlServerOptions => {
							sqlServerOptions.UseNetTopologySuite();
						})
						.UseInternalServiceProvider(p))
				.BuildServiceProvider();

			return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
		}
	}
}
