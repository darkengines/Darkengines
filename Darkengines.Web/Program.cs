using Darkengines;
using Darkengines.Web;
using Darkengines;
using Darkenginess.Security;
using Darkengines.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Darkengines.Applications;
using Darkengines.Data;
using Darkengines.Authentication;
using Darkengines.Authentication.Jwt;

var applicationBuilder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Environment: {applicationBuilder.Environment.EnvironmentName}");

applicationBuilder.Configuration.AddJsonFile("appsettings.json");
applicationBuilder.Configuration.AddJsonFile($"appsettings.{applicationBuilder.Environment.EnvironmentName}.json", optional: true);
applicationBuilder.Host.UseSerilog((context, loggerConfiguration) =>
	loggerConfiguration.ReadFrom.Configuration(context.Configuration)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.ApplicationName), context.HostingEnvironment.ApplicationName)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.EnvironmentName), context.HostingEnvironment.EnvironmentName)
);
var interceptorAdded = false;
applicationBuilder.Services
	.AddHsts(options => { })
	.AddDarkengines(applicationBuilder.Configuration)
	.AddCors()
	.AddScoped<HttpIdentityProvider>()
	.AddSingleton<ITraceWriter, SerilogTraceWriter>()
	.AddScoped<IIdentityProvider>(serviceProvider => serviceProvider.GetRequiredService<HttpIdentityProvider>())
	.AddScoped<IApplicationContext, WebApplicationContext>()
	.AddDbContext<ApplicationDbContext>((serviceProvider, options) => {
		var model = serviceProvider.GetRequiredService<IModel>();
		var interceptors = serviceProvider.GetRequiredService<IEnumerable<IInterceptor>>();
		var configuration = serviceProvider.GetRequiredService<IConfiguration>();
		if (!interceptorAdded) {
			options.AddInterceptors(interceptors);
			interceptorAdded = true;
		}
		options.UseInternalServiceProvider(serviceProvider);
		options.UseSqlServer(configuration.GetConnectionString("default"), sqlServerOptions => {
			sqlServerOptions.CommandTimeout(60);
			sqlServerOptions.UseNetTopologySuite();
		});
		options.UseModel(model);
		options.EnableSensitiveDataLogging();
	})
	.AddControllers();

var application = applicationBuilder.Build();

application.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
application.UseWebSockets();
application.MapControllers();
application.UseDarkengines();

await application.RunAsync();