using Darkengines.Core.Authentication;
using Darkengines.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Security {
	public class Identity : IIdentity {
		public Identity(User user) {
			User = user;
		}
		public User User { get; }
	}
}
