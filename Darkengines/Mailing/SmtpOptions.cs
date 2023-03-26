using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Mailing {
	public class SmtpOptions {
		public string Host { get; set; }
		public int Port { get; set; }
		public NetworkCredential? Credentials { get; set; }
		public bool UseSsl { get; set; }
		public string DisplayName { get; set; }
		public string SenderEmailAddress { get; set; }
	}
}
