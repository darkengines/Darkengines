using Darkengines.Authentication;
using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Security {
	public class Identity : IIdentity {
		public Identity(User user) {
			User = user;
		}
		public User User { get; }
	}
}
