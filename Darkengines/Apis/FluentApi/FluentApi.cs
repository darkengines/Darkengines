using Darkengines.Authentication;
using Darkengines.Data;
using Darkengines.Expressions.Converters;
using Darkengines.Models;
using Esprima;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Apis.FluentApi {
    public class FluentApi {
        protected JsonSerializer JsonSerializer { get; }
        protected ConversionContext ConverterContext { get; }
        protected ILogger<FluentApi> Logger { get; }

        public FluentApi(
            JsonSerializer jsonSerializer,
            ConversionContext converterContext,
            ILogger<FluentApi> logger
        ) {
            JsonSerializer = jsonSerializer;
            Logger = logger;
            var extensionTypes = new[] { typeof(Queryable), typeof(Enumerable), typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions) };
            var typeIdentifiers = new Dictionary<string, Type>() { };
            converterContext.TypeIdentifiers = typeIdentifiers;
            converterContext.ExtensionTypes = extensionTypes;
            ConverterContext = converterContext;
        }

        public async Task<FluentApiResult> ExecuteAsync(string source, FluentApiContext context) {
            var scope = new ConverterScope() {
                Identifiers = context.Identifiers,
            };

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
            var apiResult = new FluentApiResult {
                HasResult = hasResult,
                Result = result,
            };
            return apiResult;
        }
    }
}
