using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Mailing {
	public class SmtpClientFactory {
		protected IOptions<SmtpOptions> Options { get; }
		public SmtpClientFactory(IOptions<SmtpOptions> options) {
			Options = options;
		}
		public async Task<SmtpClient> CreateSmtpClient() {
			var smtpClient = new SmtpClient();
			await smtpClient.ConnectAsync(Options.Value.Host, Options.Value.Port, Options.Value.UseSsl);
			if (Options.Value.Credentials != null) {
				var credentials = new NetworkCredential(Options.Value.Credentials.UserName, Options.Value.Credentials.Password);
				await smtpClient.AuthenticateAsync(credentials);
			}
			return smtpClient;
		}
	}
}
