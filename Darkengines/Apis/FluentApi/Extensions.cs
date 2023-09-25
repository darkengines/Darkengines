using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Apis.FluentApi {
    public static class Extensions {
        public static IServiceCollection AddFluentApi(this IServiceCollection services) {
            return services.AddSingleton<FluentApi>();
        }
    }
}
