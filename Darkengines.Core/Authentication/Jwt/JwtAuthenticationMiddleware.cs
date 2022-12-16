using Darkengines.Core.Security;
using Darkengines.Core.Users.Entities;
using Darkengines.Cores.Security;
using Darkengines.Expressions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Authentication.Jwt {
	public class JwtAuthenticationMiddleware {
		protected JwtAuthenticationConfiguration JwtAuthenticationConfiguration { get; }
		RequestDelegate Next { get; }
		public JwtAuthenticationMiddleware(
			RequestDelegate next,
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration
		) {
			Next = next;
			JwtAuthenticationConfiguration = jwtAuthenticationConfiguration;
		}

		public async Task Invoke(HttpContext context, HttpIdentityProvider httpIdentityProvider, IIdentityProvider identityProvider) {
			var idToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			if (idToken != null) {
				JwtAuthenticationConfiguration.JwtSecurityTokenHandler.ValidateToken(
					idToken,
					JwtAuthenticationConfiguration.Parameters,
					out var securityToken
				);
				var jwt = new JwtSecurityToken(idToken);
				var user = (User)jwt.Payload["user"];
				httpIdentityProvider.Identity = new Identity(user);
			} else {
				httpIdentityProvider.Identity = new Identity(null);
			}
			await Next(context);
		}
	}
}
