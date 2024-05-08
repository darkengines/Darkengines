using System;

namespace Darkengines.Upload {
	public class UploadSocketContext {
		public Guid? Id { get; }
		public UploadSocket Socket { get; }
		public UploadSocketContext(UploadSocket uploadSocket, Guid? id = null) {
			Socket = uploadSocket;
			Id = id;
		}
	}
}
