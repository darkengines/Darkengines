﻿using Darkengines.Apis.FluentApi;
using Darkengines.Expressions;
using Darkengines.Authentication;
using Darkengines.Data;
using Darkengines.Expressions.Converters;
using Darkengines.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Operation.Buffer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Darkengines.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using Darkengines.UserGroups.Entities;
using System.Collections.ObjectModel;

namespace Darkengines.WebSockets {
	public class MessagingClient {
		public Guid ConnectionId { get; }
		public User Identity { get; }
		public WebSocket WebSocket { get; }
		protected JsonSerializer JsonSerializer { get; }
		protected FluentApi FluentApi { get; }
		protected IServiceProvider ServiceProvider { get; }
		protected ApplicationDbContext ApplicationDbContext { get; }
		protected ModelProvider ModelProvider { get; }
		protected Mutation Mutation { get; }
		protected MessagingSystem MessagingSystem { get; }

		public MessagingClient(
			Guid connectionId,
			User identity,
			WebSocket webSocket,
			JsonSerializer jsonSerializer,
			FluentApi fluentApi,
			IServiceProvider serviceProvider,
			ApplicationDbContext context,
			ModelProvider modelProvider,
			Mutation mutation,
			MessagingSystem messagingSystem
		) {
			ConnectionId = connectionId;
			Identity = identity;
			WebSocket = webSocket;
			JsonSerializer = jsonSerializer;
			FluentApi = fluentApi;
			ServiceProvider = serviceProvider;
			ApplicationDbContext = context;
			ModelProvider = modelProvider;
			Mutation = mutation;
			MessagingSystem = messagingSystem;
		}
		public async Task Start() {
			var buffer = new byte[1024];
			while (!WebSocket.CloseStatus.HasValue) {
				try {
					var message = await ReceiveMessageAsync(WebSocket, buffer);
					await ProcessMessage(WebSocket, message);
				} catch (Exception exception) {
					using var writer = new StringWriter();
					using var jsonWriter = new JsonTextWriter(writer);
					JsonSerializer.Serialize(jsonWriter, exception);
					var bytes = Encoding.UTF8.GetBytes(writer.ToString());
					await SendMessageAsync(WebSocket, bytes);
				}
			}
		}
		public async Task KeepAlive() {
			var client = await ApplicationDbContext.Clients.FindAsync(ConnectionId);
			client!.LastKeepAliveDateTime = DateTimeOffset.UtcNow;
			await ApplicationDbContext.SaveChangesAsync();
		}
		public async Task ProcessMessage(WebSocket webSocket, string message) {
			if (!string.IsNullOrEmpty(message)) {
				using (var textReader = new StringReader(message)) {
					using (var jsonReader = new JsonTextReader(textReader)) {
						var messageRequest = JsonSerializer.Deserialize<MessageRequest>(jsonReader);
						using var scope = ServiceProvider.CreateAsyncScope();

						var queries = ApplicationDbContext.Model.GetEntityTypes().Select(entityType => {
							return new KeyValuePair<string, Expression>(entityType.ShortName(), Expression.Constant(ApplicationDbContext.GetQuery(entityType.ClrType)));
						});

						var identifiers = new Dictionary<string, Expression>() {
							{ nameof(ModelProvider), Expression.Constant(ModelProvider) },
							{ nameof(Mutation), Expression.Constant(Mutation) },
							{ nameof(MessagingClient), Expression.Constant(this) }
						};
						foreach (var query in queries) identifiers.Add(query.Key, query.Value);

						var fluentApiContext = new FluentApiContext();
						foreach (var identifier in identifiers) {
							fluentApiContext.Identifiers.Add(identifier);
						}
						var response = new MessageResponse {
							Id = messageRequest.Id
						};
						try {
							var fluentApiResult = await FluentApi.ExecuteAsync(messageRequest.Request.Content, fluentApiContext);
							response.Response = new Response();
							if (fluentApiResult.HasResult) {
								response.Response.Content = fluentApiResult.Result;
							};
							response.Success = true;
							using (var textWriter = new StringWriter()) {
								using (var jsonWriter = new JsonTextWriter(textWriter)) {
									JsonSerializer.Serialize(jsonWriter, response);
									var bytes = Encoding.UTF8.GetBytes(textWriter.ToString());
									await SendMessageAsync(webSocket, bytes);
								}
							}
						} catch (Exception e) {
							var result = new MessageResponse() {
								Id = messageRequest.Id,
								Success = false,
								Response = null
							};

							using (var textWriter = new StringWriter()) {
								using (var jsonWriter = new JsonTextWriter(textWriter)) {
									JsonSerializer.Serialize(jsonWriter, result);
									var bytes = Encoding.UTF8.GetBytes(textWriter.ToString());
									await SendMessageAsync(webSocket, bytes);
								}
							}
						}
					}
				}
			}
		}

		public async Task<string> ReceiveMessageAsync(WebSocket webSocket, byte[] buffer) {
			var stringBuilder = new StringBuilder();
			WebSocketReceiveResult receiveResult;
			do {
				receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
				stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));
			} while (!receiveResult.EndOfMessage);
			return stringBuilder.ToString();
		}

		public async Task SendMessageAsync(WebSocket webSocket, byte[] data) {
			await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}
}
