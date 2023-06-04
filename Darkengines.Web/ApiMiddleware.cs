using Darkengines.Authentication;
using Darkengines.Data;
using Darkengines.Models;
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

namespace Darkengines.Web {
    //class UserPermission {
    //	public int Id { get; set; }
    //	public bool SelfPermission { get; set; }
    //	public bool HashedPasswordPermission { get; set; }
    //}
    public class ApiMiddleware {
        protected RequestDelegate Next { get; }
        protected JsonSerializer JsonSerializer { get; }
        protected ConversionContext ConverterContext { get; }
        protected ModelProvider ModelProvider { get; }
        protected ILogger<ApiMiddleware> Logger { get; }

        public ApiMiddleware(
            RequestDelegate next,
            JsonSerializer jsonSerializer,
            ConversionContext converterContext,
            ModelProvider modelProvider,
            ILogger<ApiMiddleware> logger
        ) {
            Next = next;
            ModelProvider = modelProvider;
            JsonSerializer = jsonSerializer;
            Logger = logger;
            var extensionTypes = new[] { typeof(Queryable), typeof(Enumerable), typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions) };
            var typeIdentifiers = new Dictionary<string, Type>() { };
            converterContext.TypeIdentifiers = typeIdentifiers;
            converterContext.ExtensionTypes = extensionTypes;
            ConverterContext = converterContext;
        }
        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext applicationDbContext,
            Authentication.Authentication authentication,
            Mutation mutation
        ) {
            var source = context.Request.Path.Value;
            if (!string.IsNullOrWhiteSpace(source)) source = source.Substring(1);

            var queries = applicationDbContext.Model.GetEntityTypes().Select(entityType => {
                return new KeyValuePair<string, Expression>(entityType.ShortName(), Expression.Constant(applicationDbContext.GetQuery(entityType.ClrType)));
            });

            var identifiers = new Dictionary<string, Expression>() {
                { "Numbers", Expression.Constant(Enumerable.Range(0, 100)) },
                { nameof(ApplicationDbContext.Users), Expression.Constant(applicationDbContext.Users) },
                { nameof(ApplicationDbContext.UserUserGroups), Expression.Constant(applicationDbContext.UserUserGroups) },
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
            var javascriptParser = new JavaScriptParser(new ParserOptions() { Tokens = true });
            var tree = javascriptParser.ParseScript(source, source).Body.First() as Esprima.Ast.ExpressionStatement;
            var root = tree.Expression;

            //var tree = CSharpSyntaxTree.ParseText(source);
            //var root = await tree.GetRootAsync();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var conversionResult = ConverterContext.Convert("Javascript", root, scope);
            stopwatch.Stop();
            Logger.LogInformation($"Input expression convertion duration: ${stopwatch.Elapsed}");
            var expression = conversionResult.Expression!;
            var result = default(object);
            var hasResult = default(bool);

            if (conversionResult.IsAsynchronous) {
                var lambda = Expression.Lambda<Func<dynamic>>(expression);
                var func = lambda.Compile();
                hasResult = expression.Type != typeof(Task);
                if (hasResult) {
                    result = await func();
                } else {
                    await func();
                }
            } else {
                var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)));
                var func = lambda.Compile();
                hasResult = func.Method.ReturnType != typeof(void);
                if (hasResult) {
                    result = func();
                } else {
                    func();
                }
            }
            if (hasResult) {
                using (var stringWriter = new StringWriter()) {
                    JsonSerializer.Serialize(stringWriter, result);
                    await context.Response.WriteAsync(stringWriter.ToString());
                }
            }
            //await Next(context);
        }

    }
}
