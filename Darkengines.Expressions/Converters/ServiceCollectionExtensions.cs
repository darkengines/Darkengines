using Darkengines.Expressions.Converters.CSharp;
using Darkengines.Expressions.Converters.Javascript;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddConverters(this IServiceCollection serviceCollection) {
			return serviceCollection.AddCSharpConverters().AddJavascriptConverters().AddSingleton(new AnonymousTypeBuilder("Darkengines.Expressions", "Anonymous")).AddSingleton<LambdaExpressionConverter>();
		}
	}
}
