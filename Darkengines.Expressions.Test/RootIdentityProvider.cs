using Darkengines.Authentication;
using Darkengines.Users.Entities;
using Darkengines.Applications;
using Darkengines.Data;
using Darkengines.Security;
using Darkengines.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Test
{
    public class RootIdentityProvider : IIdentityProvider {
		protected IIdentity Identity { get; }
		public RootIdentityProvider(User rootUser) {
			Identity = new Identity(rootUser);
		}
		public async Task<IIdentity> GetIdentityAsync() {
			return Identity;
		}

		public IIdentity GetIdentity() {
			return Identity;
		}
	}
}