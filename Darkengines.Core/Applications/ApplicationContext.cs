using Darkengines.Core.Authentication;
using Darkengines.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Applications {
	public class ApplicationContext : IApplicationContext {
		IIdentityProvider IdentityProvider { get; }
		public User CurrentUser {
			get { return IdentityProvider.GetIdentity().User; }
		}
		public ApplicationContext(IIdentityProvider identityProvider) {
			IdentityProvider = identityProvider;
		}
	}
}
