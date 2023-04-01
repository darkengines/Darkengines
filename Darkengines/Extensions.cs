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

namespace Darkengines {
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
            serviceCollection.AddJsonSerializer();
            serviceCollection.AddMailing(configuration);
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
                    (serviceProvider, dbContextOptionsBuilder) => {
                        dbContextOptionsBuilder.AddInterceptors(serviceProvider.GetRequiredService<IEnumerable<IInterceptor>>());
                        dbContextOptionsBuilder.UseSqlServer("Server=.", sqlServerOptions => {
                            sqlServerOptions.UseNetTopologySuite();
                        })
                        .UseInternalServiceProvider(serviceProvider);
                    })
                .BuildServiceProvider();

            return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
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
    }
}
