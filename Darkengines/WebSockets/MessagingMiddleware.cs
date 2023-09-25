using Darkengines.Authentication.Jwt;
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

namespace Darkengines.WebSockets {
    public class MessagingMiddleware {
        protected RequestDelegate Next { get; }
        protected Messaging Messaging { get; }
        public MessagingMiddleware(RequestDelegate next, Messaging messaging) {
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
            Mutation mutation
        ) {
            var identity = identityProvider.GetIdentity();
            if (identity != null) {

                if (context.WebSockets.IsWebSocketRequest) {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var client = new MessagingClient(identity, webSocket, jsonSerializer, fluentApi, serviceProvider, applicationDbContext, modelProvider, mutation);
                    try {
                        Messaging.AddClient(client);
                        await client.Start();
                    } catch (Exception e) {
                        throw;
                    } finally {
                        Messaging.RemoveClient(client);
                    }
                } else {
                    context.Response.StatusCode = 400;
                }
            }
        }
    }
}
