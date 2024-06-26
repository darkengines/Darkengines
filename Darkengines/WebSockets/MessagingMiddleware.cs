﻿using Darkengines.Authentication.Jwt;
using Darkengines.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Darkengines.Apis.FluentApi;
using Darkengines.Data;
using Darkengines.Models;
using Darkengines.Users.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Darkengines.Messaging.Entities;
using Darkengines.Applications.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Darkengines.WebSockets {
	public class MessagingMiddleware {
		protected RequestDelegate Next { get; }
		protected MessagingSystem Messaging { get; }
		public MessagingMiddleware(RequestDelegate next, MessagingSystem messaging) {
			Next = next;
			Messaging = messaging;
		}
		public async Task Invoke(
			HttpContext context,
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration,
			IIdentityProvider identityProvider,
			JsonSerializer jsonSerializer,
			FluentApi fluentApi,
			ApplicationDbContext applicationDbContext,
			IServiceProvider serviceProvider,
			ModelProvider modelProvider,
			Mutation mutation,
			MessagingSystem messagingSystem
		) {
			if (context.WebSockets.IsWebSocketRequest) {
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
					var clientEntity = default(Client);
					var connectionId = Guid.NewGuid();
					var webSocket = await context.WebSockets.AcceptWebSocketAsync();
					var client = new MessagingClient(
						connectionId,
						currentUser,
						webSocket,
						jsonSerializer,
						fluentApi,
						serviceProvider,
						applicationDbContext,
						modelProvider,
						mutation,
						messagingSystem
					);
					try {
						Messaging.AddClient(client);
						clientEntity = new Client {
							ConnectionId = connectionId,
							Host = new Uri(@"https://localhost/api"),
							UserId = currentUser.Id,
							LastKeepAliveDateTime = DateTimeOffset.UtcNow,
						};
						await applicationDbContext.Clients.AddAsync(clientEntity);
						await applicationDbContext.SaveChangesAsync();
						await client.Start();
					} finally {
						if (clientEntity != default) {
							applicationDbContext.Clients.Remove(clientEntity);
							await applicationDbContext.SaveChangesAsync();
						}
						Messaging.RemoveClient(client);
					}
				} else {
					context.Response.StatusCode = 400;
				}
				return;
			}
			await Next(context);
		}
	}
}
