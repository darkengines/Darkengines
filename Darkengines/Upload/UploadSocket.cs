using Darkengines.Users.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Darkengines.Upload {
	public class UploadSocket {
		protected JsonSerializer Serializer { get; }
		protected IUploadHandler UploadHandler { get; }

		public UploadSocket(User user, WebSocket webSocket,
			HttpContext httpContext,
			JsonSerializer serializer, IUploadHandler uploadHandler) {
			User = user;
			WebSocket = webSocket;
			HttpContext = httpContext;
			Serializer = serializer;
			UploadHandler = uploadHandler;
		}
		public User User { get; }
		public WebSocket WebSocket { get; }
		public HttpContext HttpContext { get; }
		public async Task<byte[]> ReadToEnd() {
			var bufferSize = 65536;
			var buffer = new Memory<byte>(new byte[bufferSize]);
			byte[] bytes = null;
			var endOfMessage = false;
			var received = 0;
			using (var memoryStream = new MemoryStream()) {
				while (!endOfMessage) {
					var result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
					await memoryStream.WriteAsync(buffer.ToArray(), 0, result.Count);
					received += result.Count;
					if (received > UploadHandler.MaximumFileSize) throw new Exception("Maximum file size exceeded");
					endOfMessage = result.EndOfMessage;
				}
				bytes = memoryStream.ToArray();
			}
			return bytes;
		}
		public async Task SendMessageAsync(object payload, Guid? id = null) {
			var message = new UploadSocketMessage() {
				Id = id,
				Content = JToken.FromObject(payload, Serializer),
				Error = null
			};
			using (var textWriter = new StringWriter()) {
				Serializer.Serialize(textWriter, message);
				var bytes = Encoding.UTF8.GetBytes(textWriter.ToString());
				await WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, new CancellationToken());
			}
		}
		public async Task Start() {
			using var stream = new WebSocketStream(WebSocket);
			var uri = await UploadHandler.ProcessFile(User, stream);
			await SendMessageAsync(new { Uri = uri });
			await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Success", CancellationToken.None);
		}
	}
}
