using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Darkengines.Upload {
	public static class UploadSocketExtensions {
		public static IApplicationBuilder UseUploadSocket(this IApplicationBuilder applicationBuilder) {
			var routeBuilder = new RouteBuilder(applicationBuilder);
			routeBuilder.MapMiddlewareGet("/upload", app => {
				app.UseWebSockets();
				app.UseMiddleware<UploadSocketMiddleware>();
			});
			return applicationBuilder.UseRouter(routeBuilder.Build());
		}
		public static IServiceCollection AddUploadSocket(this IServiceCollection services) {
			return services.AddSingleton<ConcurrentDictionary<UploadSocket, UploadSocket>>();
		}
	}
}
