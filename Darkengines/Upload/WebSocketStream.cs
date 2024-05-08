using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Upload {
	public class WebSocketStream : Stream {
		protected WebSocket WebSocket { get; }
		protected bool EndOfMessage { get; set; }
		public WebSocketStream(WebSocket webSocket) {
			WebSocket = webSocket;
		}
		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => throw new NotImplementedException();

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override void Flush() {
			throw new NotImplementedException();
		}
		public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
			if (EndOfMessage) return 0;
			var span = new ArraySegment<byte>(buffer, offset, count);
			var result = await WebSocket.ReceiveAsync(span, cancellationToken);
			EndOfMessage = result.EndOfMessage;
			return result.Count;
		}
		public override int Read(byte[] buffer, int offset, int count) {
			var readTask = this.ReadAsync(buffer, offset, count);
			readTask.Wait();
			var result = readTask.Result;
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotImplementedException();
		}

		public override void SetLength(long value) {
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotImplementedException();
		}
	}
}
