using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Authentication.Jwt {
	public class EmailAlreadyInUseException : Exception {
		public EmailAlreadyInUseException() {
		}

		public EmailAlreadyInUseException(string message) : base(message) {
		}

		public EmailAlreadyInUseException(string message, Exception innerException) : base(message, innerException) {
		}

		protected EmailAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
