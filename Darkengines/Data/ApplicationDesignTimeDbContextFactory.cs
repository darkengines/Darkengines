using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Darkengines.Data {
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
		public ApplicationDbContext CreateDbContext(string[] args) {
			return CreateDbContext(true);
		}
		public ApplicationDbContext CreateDbContext(bool isDesignTime = false) {

			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json");
			configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
			var configuration = configurationBuilder.Build();

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddEntityFrameworkSqlServer();
			serviceCollection.AddEntityFrameworkSqlServerNetTopologySuite();
			serviceCollection.AddDarkenginesModel(isDesignTime);
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
