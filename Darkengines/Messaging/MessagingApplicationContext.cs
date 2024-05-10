using Darkengines.Applications;
using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Messaging {
	internal class MessagingApplicationContext : IApplicationContext {
		public User CurrentUser { get; }
		public MessagingApplicationContext(User user) {
			CurrentUser = user;
		}
	}
}
