using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddJavascriptConverters(this IServiceCollection serviceCollection) {
            return serviceCollection
                .AddSingleton<IConverter, ArrowFunctionExpressionConverter>()
                .AddSingleton<IConverter, BinaryExpressionConverter>()
                .AddSingleton<IConverter, IdentifierConverter>()
                .AddSingleton<IConverter, CallExpressionConverter>((serviceProvider) => {
                    var callExpressionConverter = new CallExpressionConverter();
                    callExpressionConverter.ExpressionResover = (MethodInfo methodInfo, Expression memberExpression, IEnumerable<Expression> argumentExpressions) => {
                        var memberLikeMethodInfo = ExpressionHelper.ExtractMethodInfo<Func<string, string, bool>>(() => StringExtensions.Like);
                        var likeMethodInfo = ExpressionHelper.ExtractMethodInfo<Func<string, string, bool>>(() => EF.Functions.Like);
                        if (methodInfo == memberLikeMethodInfo) {
                            return callExpressionConverter.ResolveExpression(likeMethodInfo, null, argumentExpressions.Prepend(Expression.Constant(EF.Functions)));
                        }
                        return callExpressionConverter.ResolveExpression(methodInfo, memberExpression, argumentExpressions);
                    };

                    return callExpressionConverter;
                })
                .AddSingleton<IConverter, LiteralConverter>()
                .AddSingleton<IConverter, LogicalExpressionConverter>()
                .AddSingleton<IConverter, UnaryExpressionConverter>()
                .AddSingleton<IConverter, MemberExpressionConverter>()
                .AddSingleton<IConverter, StaticMemberExpressionConverter>()
                .AddSingleton<IConverter, ObjectExpressionConverter>();
        }
    }
}
