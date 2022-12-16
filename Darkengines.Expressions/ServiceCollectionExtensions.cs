using Darkengines.Expressions.Converters;
using Darkengines.Expressions.Converters.CSharp;
using Darkengines.Expressions.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddExpressions(this IServiceCollection serviceCollection) {
			return serviceCollection.AddConverters().AddSingleton<ConversionContext>().AddSingleton<PermissionEntityTypeBuilder>();
		}
	}
}
