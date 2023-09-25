using Darkengines;
using Darkengines.Web;
using Darkengines;
using Darkengines.Applications;
using Darkengines.Authentication;
using Darkengines.Authentication.Jwt;
using Darkengines.Data;
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

var applicationBuilder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Environment: {applicationBuilder.Environment.EnvironmentName}");

applicationBuilder.Configuration.AddJsonFile("appsettings.json");
applicationBuilder.Configuration.AddJsonFile($"appsettings.{applicationBuilder.Environment.EnvironmentName}.json", optional: true);
applicationBuilder.Host.UseSerilog((context, loggerConfiguration) =>
	loggerConfiguration.ReadFrom.Configuration(context.Configuration)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.ApplicationName), context.HostingEnvironment.ApplicationName)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.EnvironmentName), context.HostingEnvironment.EnvironmentName)
);
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
		options.AddInterceptors(interceptors);
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

application.MapControllers();
application.Map("/api", apiApplication => {
	apiApplication.UseExceptionHandler(exceptionHandlerApplication => {
		exceptionHandlerApplication.Run(async context => {
			var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
			if (exceptionHandlerPathFeature?.Error != null) {
				var jsonSerializer = application.Services.GetService<JsonSerializer>();
				using var writer = new StreamWriter(context.Response.BodyWriter.AsStream());
				using var jsonWriter = new JsonTextWriter(writer);
				jsonSerializer.Serialize(jsonWriter, exceptionHandlerPathFeature.Error);
			}
		});
	});
	apiApplication.UseMiddleware<JwtAuthenticationMiddleware>();
	apiApplication.UseMiddleware<ApiMiddleware>();
});

await application.RunAsync();