using Darkengines.Core.Applications;
using Darkengines.Core.Authentication;
using Darkengines.Core.Data;
using Darkengines.Core.Security;
using Darkengines.Core.Users;
using Darkengines.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Test {
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