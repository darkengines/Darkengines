using Darkengines.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Templating {
	public static class Extensions {
		public static IServiceCollection AddTemplating(this IServiceCollection serviceCollection, Func<RazorLightEngineBuilder, RazorLightEngineBuilder> configure) {
			return serviceCollection.AddSingleton(serviceProvider => {
				var engineBuilder = new RazorLightEngineBuilder();
				var engine = configure(engineBuilder).Build();
				return engine;
			});
		}
	}
}
