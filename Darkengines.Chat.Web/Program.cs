using Darkengines.Chat.Web;
using Darkengines.Core;
using Darkengines.Core.Applications;
using Darkengines.Core.Authentication;
using Darkengines.Core.Authentication.Jwt;
using Darkengines.Core.Data;
using Darkengines.Cores.Security;
using Darkengines.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Serilog;

var applicationBuilder = WebApplication.CreateBuilder(args);
applicationBuilder.Configuration.AddJsonFile("appsettings.json");
applicationBuilder.Host.UseSerilog((context, loggerConfiguration) =>
	loggerConfiguration.ReadFrom.Configuration(context.Configuration)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.ApplicationName), context.HostingEnvironment.ApplicationName)
	.Enrich.WithProperty(nameof(context.HostingEnvironment.EnvironmentName), context.HostingEnvironment.EnvironmentName)
);

applicationBuilder.Services
	.AddHsts(options => { })
	.AddDarkengines()
	.AddCors()
	.AddScoped<HttpIdentityProvider>()
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
	});

var application = applicationBuilder.Build();

application.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

application.Map("/api", apiApplication => {
	apiApplication.UseExceptionHandler(exceptionHandlerApplication => {
		exceptionHandlerApplication.Run(async context => {
			var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
			if (exceptionHandlerPathFeature?.Error != null) {
				var serializedError = JsonConvert.SerializeObject(exceptionHandlerPathFeature.Error);
				await context.Response.WriteAsync(serializedError);
			}
		});
	});
	apiApplication.UseMiddleware<JwtAuthenticationMiddleware>();
	apiApplication.UseMiddleware<ApiMiddleware>();
});

await application.RunAsync();