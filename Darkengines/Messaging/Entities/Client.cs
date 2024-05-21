using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Messaging.Entities {
	public class Client {
		public required Uri Host { get; set; }
		public required Guid ConnectionId { get; set; }
		public required int UserId { get; set; }
		public virtual User? User { get; set; }
		public required DateTimeOffset LastKeepAliveDateTime { get; set; }
	}
}
