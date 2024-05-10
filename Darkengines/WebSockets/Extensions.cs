using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.WebSockets {
    public static class Extensions {
        public static IServiceCollection AddMessaging(this IServiceCollection services) {
            return services.AddSingleton<MessagingSystem>();
        }
		public static IApplicationBuilder UseMessaging(this IApplicationBuilder applicationBuilder) {
			return applicationBuilder.UseMiddleware<MessagingMiddleware>();
		}
	}
}
