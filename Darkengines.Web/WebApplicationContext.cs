using Darkengines.Data;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkengines.Applications;
using Darkengines.Authentication;
using Darkengines.Users.Entities;

namespace Darkengines.Web
{
    public class WebApplicationContext : IApplicationContext {
		protected IIdentityProvider IdentityProvider { get; }
		public User CurrentUser => IdentityProvider.GetIdentity()?.User;

		public WebApplicationContext(IIdentityProvider identityProvider) {
			IdentityProvider = identityProvider;
		}
	}
}
