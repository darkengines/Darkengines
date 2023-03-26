using Darkengines.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Mailing {
	public static class Extensions {
		public static IServiceCollection AddMailing(this IServiceCollection serviceCollection, IConfiguration configuration) {
			serviceCollection.Configure<SmtpOptions>(options => { configuration.GetSection(nameof(SmtpClient)).Bind(options); });
			return serviceCollection.AddSingleton<SmtpClientFactory>();
		}
	}
}
