using Darkengines.Users.Entities;
using Darkengines.Expressions;
using Darkengines.Expressions.Converters;
using Esprima;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.SignalR.Protocol;
using Darkengines.Apis.FluentApi;
using Microsoft.EntityFrameworkCore.Metadata;
using Darkengines.Models;
using Darkengines.Data;

namespace Darkengines.Web {
    public class ApiMiddleware {
        protected RequestDelegate Next { get; }
        protected JsonSerializer JsonSerializer { get; }
        protected ConversionContext ConverterContext { get; }
        protected ModelProvider ModelProvider { get; }
        protected ILogger<ApiMiddleware> Logger { get; }
        protected FluentApi FluentApi { get; }

        public ApiMiddleware(
            RequestDelegate next,
            JsonSerializer jsonSerializer,
            ConversionContext converterContext,
            ModelProvider modelProvider,
            FluentApi fluentApi,
            ILogger<ApiMiddleware> logger
        ) {
            Next = next;
            ModelProvider = modelProvider;
            JsonSerializer = jsonSerializer;
            Logger = logger;
            var extensionTypes = new[] { typeof(Queryable), typeof(Enumerable), typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions), typeof(Expressions.StringExtensions) };
            converterContext.ExtensionTypes = extensionTypes;
            ConverterContext = converterContext;
            FluentApi = fluentApi;
        }
        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext applicationDbContext,
            Darkengines.Authentication.Authentication authentication,
            IModel model,
            Mutation mutation
        ) {

            var typeIdentifiers = model.GetEntityTypes().ToDictionary(entityType => entityType.ClrType.Name, entityType => entityType.ClrType);
            ConverterContext.TypeIdentifiers = typeIdentifiers;

            var source = context.Request.Path.Value;
            if (!string.IsNullOrWhiteSpace(source)) source = source.Substring(1);

            var queries = applicationDbContext.Model.GetEntityTypes().Select(entityType => {
                return new KeyValuePair<string, Expression>(entityType.ShortName(), Expression.Constant(applicationDbContext.GetQuery(entityType.ClrType)));
            });

            var identifiers = new Dictionary<string, Expression>() {
                { nameof(ModelProvider), Expression.Constant(ModelProvider) },
                { nameof(Authentication), Expression.Constant(authentication) },
                { nameof(Mutation), Expression.Constant(mutation) }
            };
            foreach (var query in queries) identifiers.Add(query.Key, query.Value);

            var scope = new ConverterScope() {
                Identifiers = identifiers,
            };

            if (context.Request.Method == HttpMethod.Post.Method) {
                using (var reader = new StreamReader(context.Request.Body)) {
                    source = await reader.ReadToEndAsync();
                }
            }

            var stopwatch = new Stopwatch();
            var fluentApiContext = new FluentApiContext();
            foreach (var identifier in identifiers) fluentApiContext.Identifiers.Add(identifier);
            stopwatch.Start();
            var result = await FluentApi.ExecuteAsync(source, fluentApiContext);
            if (result.HasResult) {
                using (var stringWriter = new StringWriter()) {
                    JsonSerializer.Serialize(stringWriter, result.Result);
                    await context.Response.StartAsync();
                    await context.Response.WriteAsync(stringWriter.ToString());
                }
            }
            stopwatch.Stop();
            Logger.LogInformation($"Request processed in ${stopwatch.Elapsed}");

            //await Next(context);
        }

    }
}
