using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Serialization {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddJsonSerializer(this IServiceCollection serviceCollection) {
			var serializer = new JsonSerializer() {
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Objects,
				Formatting = Formatting.Indented,
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
			};
			serviceCollection.AddSingleton(serviceProvider => {
				var traceWriter = serviceProvider.GetRequiredService<ITraceWriter>();
				serializer.TraceWriter = traceWriter;
				return serializer;
			});
			return serviceCollection;
		}
	}
}
