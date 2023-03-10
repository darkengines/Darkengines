using Microsoft.Extensions.DependencyInjection;

namespace Darkengines.Models {
	public static class Extensions {
		public static IServiceCollection AddModels(this IServiceCollection serviceCollection) {
			return serviceCollection.AddSingleton<ModelProvider>();
		}
	}
}
