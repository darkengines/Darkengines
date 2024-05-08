using Darkengines.Authentication.Jwt;
using Darkengines.Data;
using Darkengines.Users.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace Darkengines.Upload {
	public class UploadSocketMiddleware {
		protected RequestDelegate Next { get; }

		public UploadSocketMiddleware(RequestDelegate next) {
			Next = next;
		}
		public async Task<Task> Invoke(HttpContext context,
			ApplicationDbContext applicationDbContext,
			ConcurrentDictionary<UploadSocket, UploadSocket> socketClients,
			JsonSerializer jsonSerializer,
			IEnumerable<IUploadHandler> uploadHandlers,
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration
		) {
			const string auth0Domain = "https://comeet.auth0.com/";
			var type = context.Request.Query["type"].First();
			var idToken = context.Request.Query["idToken"].First();
			var jwt = new JsonWebToken(idToken);
			SecurityToken securityToken = null;
			var tokenValidationResult = await jwtAuthenticationConfiguration.JwtSecurityTokenHandler.ValidateTokenAsync(idToken, jwtAuthenticationConfiguration.Parameters);
			context.User.AddIdentity(tokenValidationResult.ClaimsIdentity);
			User currentUser;
			using (var stringReader = new StringReader(((System.Text.Json.JsonElement)tokenValidationResult.Claims.First(c => c.Key == "user").Value).ToString())) {
				using (var jsonReader = new JsonTextReader(stringReader)) {
					currentUser = jsonSerializer.Deserialize<User>(jsonReader);
				}
			}
			if (currentUser != null) {
				if (context.WebSockets.IsWebSocketRequest) {
					var uploadHandler = uploadHandlers.First(uh => uh.CanHandle(currentUser, type));
					if (await uploadHandler.CanUpload(currentUser)) {
						var webSocket = await context.WebSockets.AcceptWebSocketAsync();
						var client = new UploadSocket(currentUser, webSocket, context, jsonSerializer, uploadHandler);
						try {
							socketClients.TryAdd(client, client);
							await client.Start();
						} catch (Exception e) {
							throw;
						} finally {
							socketClients.TryRemove(client, out client);
						}
					}
				} else {
					context.Response.StatusCode = 400;
				}
			}
			return Next(context);
		}
	}
}
