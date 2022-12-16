using Darkengines.Core.Authentication.Jwt;
using Darkengines.Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;

namespace Darkengines.Cores.Security {
	public class HttpIdentityProvider : IIdentityProvider {
		public IIdentity Identity { get; set; }
		public IIdentity GetIdentity() { return Identity; }
		public Task<IIdentity> GetIdentityAsync() { return Task.FromResult(Identity); }
	}
}
