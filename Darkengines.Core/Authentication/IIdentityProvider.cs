using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Authentication {
	public interface IIdentityProvider {
		public IIdentity GetIdentity();
		public Task<IIdentity> GetIdentityAsync();
	}
}
