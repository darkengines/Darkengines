using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.WebSockets {
    public static class Extensions {
        public static IServiceCollection AddWebSockets(this IServiceCollection services) {
            return services;
        }
    }
}
