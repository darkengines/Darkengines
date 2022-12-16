using Darkengines.Expressions.Application.Applications.Rules;
using Darkengines.Expressions.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Web.Models {
	public static class Extensions {
		public static IServiceCollection AddModels(this IServiceCollection serviceCollection) {
			return serviceCollection.AddSingleton<ModelProvider>();
		}
	}
}
