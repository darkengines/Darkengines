using Darkengines.Security;
using Darkengines.Users.Entities;
using Darkenginess.Security;
using Darkengines.Expressions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Darkengines.Authentication.Jwt {
	public class JwtAuthenticationMiddleware {
		protected JwtAuthenticationConfiguration JwtAuthenticationConfiguration { get; }
		protected JsonSerializer JsonSerializer { get; }
		RequestDelegate Next { get; }
		public JwtAuthenticationMiddleware(
			RequestDelegate next,
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration,
			JsonSerializer jsonSerializer
		) {
			JsonSerializer = jsonSerializer;
			Next = next;
			JwtAuthenticationConfiguration = jwtAuthenticationConfiguration;
		}

		public async Task Invoke(HttpContext context, HttpIdentityProvider httpIdentityProvider, IIdentityProvider identityProvider) {
			var idToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			if (idToken != null) {
				var result = await JwtAuthenticationConfiguration.JwtSecurityTokenHandler.ValidateTokenAsync(
					idToken,
					JwtAuthenticationConfiguration.Parameters
				);
				using var reader = new StringReader(((System.Text.Json.JsonElement)result.Claims["user"]).ToString());
				using var jsonReader = new JsonTextReader(reader);
				var user = JsonSerializer.Deserialize<User>(jsonReader);
				httpIdentityProvider.Identity = new Identity(user);
			} else {
				httpIdentityProvider.Identity = new Identity(null);
			}
			await Next(context);
		}
	}
}
