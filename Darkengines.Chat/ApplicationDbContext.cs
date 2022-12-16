using Darkengines.Expressions.Application.UserGroups.Entities;
using Darkengines.Expressions.Application.Users;
using Darkengines.Expressions.Application.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application {
	public class ApplicationDbContext : DbContext {
		public DbSet<User>? Users { get; set; }
		public DbSet<UserUserGroup>? UserUserGroups { get; set; }
		public ApplicationDbContext() : base() { }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
		}
	}

	public class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
		public ApplicationDbContext CreateDbContext(string[] args) {

			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			var configuration = configurationBuilder.Build();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddEntityFrameworkSqlServer();
			serviceCollection.AddEntityFrameworkSqlServerNetTopologySuite();
			serviceCollection.AddApplicationModel();
			serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, options) => {
				var model = serviceProvider.GetRequiredService<IModel>();
				options.UseInternalServiceProvider(serviceProvider);
				options.UseSqlServer(configuration.GetConnectionString("default"));
				options.UseModel(model);
				options.EnableSensitiveDataLogging();
			});
			var serviceProvider = serviceCollection.BuildServiceProvider();
			var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
			return applicationDbContext;
		}
	}
}
