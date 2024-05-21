using Darkengines.Applications;
using Darkengines.Authentication.Jwt;
using Darkengines.Models;
using Darkengines.Serialization;
using Darkengines.UserGroups;
using Darkengines.Users;
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
using Darkengines.Data;
using Darkengines.Authentication;
using Darkengines.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Darkengines.Users.Interceptors;
using Darkengines.Mailing;
using Darkengines.Templating;
using Darkengines.Users.Templates.Views;
using Darkengines.Apis.FluentApi;
using Microsoft.AspNetCore.Builder;
using Darkengines.Upload;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Darkengines.Apis;
using Darkengines.Storage;
using Darkengines.Messaging;

namespace Darkengines {
	public static class Extensions {
		public static IApplicationBuilder UseDarkengines(this IApplicationBuilder applicationBuilder) {
			applicationBuilder = applicationBuilder.Map("/api", apiApplication => {
				apiApplication.UseExceptionHandler(exceptionHandlerApplication => {
					exceptionHandlerApplication.Run(async context => {
						var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
						if (exceptionHandlerPathFeature?.Error != null) {
							var jsonSerializer = applicationBuilder.ApplicationServices.GetService<JsonSerializer>();
							using var writer = new StreamWriter(context.Response.BodyWriter.AsStream());
							using var jsonWriter = new JsonTextWriter(writer);
							jsonSerializer.Serialize(jsonWriter, exceptionHandlerPathFeature.Error);
						}
					});
				});
				apiApplication.UseMiddleware<JwtAuthenticationMiddleware>();
				apiApplication.UseMessaging();
				apiApplication.UseMiddleware<ApiMiddleware>();
			});
			applicationBuilder = applicationBuilder.Map("/upload", uploadApplication => {
				uploadApplication.UseMiddleware<UploadSocketMiddleware>();
			});
			return applicationBuilder;
		}
		public static IServiceCollection AddDarkenginesModel(this IServiceCollection serviceCollection, bool isDesignTime = false) {
			serviceCollection.AddSingleton((serviceProvider) => {
				var modelBuilder = CreateModelBuilder();
				var modelCustomizers = serviceProvider.GetServices<IModelCustomizer>();
				var modelRuntimeInitializer = serviceProvider.GetRequiredService<IModelRuntimeInitializer>();
				modelBuilder = modelBuilder.ConfigureUsers();
				modelBuilder = modelBuilder.ConfigureUserGroups();
				modelBuilder = modelBuilder.ConfigureAuthentication();
				modelBuilder = modelBuilder.ConfigureApplications();
				modelBuilder = modelBuilder.ConfigureMessaging();
				modelBuilder = modelBuilder.AddModels();

				var model = modelBuilder.Model.FinalizeModel();
				model = modelRuntimeInitializer.Initialize(model, isDesignTime);

				return model;
			});
			return serviceCollection;
		}
		public static IServiceCollection AddDarkengines(this IServiceCollection serviceCollection, IConfiguration configuration) {

			serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			serviceCollection.AddSingleton(configuration);
			serviceCollection.AddEntityFrameworkSqlServer();
			serviceCollection.AddEntityFrameworkSqlServerNetTopologySuite();
			serviceCollection.AddDarkenginesModel(false);
			serviceCollection.AddUsers();
			serviceCollection.AddUserGroups();
			serviceCollection.AddApplications();
			serviceCollection.AddModels();
			serviceCollection.AddData();
			serviceCollection.AddExpressions();
			serviceCollection.AddFluentApi();
			serviceCollection.AddJsonSerializer();
			serviceCollection.AddMailing(configuration);
			serviceCollection.Configure<StorageOptions>(options => configuration.GetSection("Darkengines").GetSection("Storage").Bind(options));
			serviceCollection.AddUploadSocket();
			serviceCollection.AddMessaging();
			serviceCollection.AddTemplating(renderEngineBuilder => {
				renderEngineBuilder
				.EnableDebugMode(true)
				.UseEmbeddedResourcesProject(typeof(EmailAddressConfirmation).Assembly)
				.UseMemoryCachingProvider();
				return renderEngineBuilder;
			});
			serviceCollection.AddJwtAuthentication(options => configuration.GetSection("Authentication.Jwt").Bind(options));

			return serviceCollection;
		}
		public static ModelBuilder CreateModelBuilder() {
			var modelBuilder = SqlServerConventionSetBuilder.CreateModelBuilder();
			return modelBuilder;
		}
	}
	public static class StringExtensions {
		public static byte[] ToLowerInvariantSHA256(this string @string) {
			return ToSHA256(@string.ToLowerInvariant());
		}
		public static byte[] ToSHA256(this string @string) {
			byte[] hashedValue = null;
			using (var sha256 = SHA256.Create()) {
				hashedValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(@string));
			}
			return hashedValue;
		}
		public static bool Like(this string @string, string pattern) {
			return EF.Functions.Like(@string, pattern);
		}
	}
}
