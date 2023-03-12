using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters.Javascript {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddJavascriptConverters(this IServiceCollection serviceCollection) {
			return serviceCollection
				.AddSingleton<IConverter, ArrowFunctionExpressionConverter>()
				.AddSingleton<IConverter, BinaryExpressionConverter>()
				.AddSingleton<IConverter, IdentifierConverter>()
				.AddSingleton<IConverter, CallExpressionConverter>()
				.AddSingleton<IConverter, LiteralConverter>()
				.AddSingleton<IConverter, LogicalExpressionConverter>()
				.AddSingleton<IConverter, UnaryExpressionConverter>()
				.AddSingleton<IConverter, MemberExpressionConverter>()
				.AddSingleton<IConverter, StaticMemberExpressionConverter>()
				.AddSingleton<IConverter, ObjectExpressionConverter>();
		}
	}
}
