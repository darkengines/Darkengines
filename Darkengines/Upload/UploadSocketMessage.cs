using Newtonsoft.Json.Linq;
using System;

namespace Darkengines.Upload {
	public class UploadSocketMessage {
		public Guid? Id { get; set; }
		public Exception Error { get; set; }
		public JToken Content { get; set; }
	}
}
