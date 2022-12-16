using Comeet.Core.Common;
using System;
using System.Runtime.Serialization;

namespace Comeet.Core.Authentication {
	public class EmailNotFoundException : NotFoundException{

		public EmailNotFoundException() { }

		public EmailNotFoundException(string message) :base ( message){ }

		public EmailNotFoundException(string message, Exception innerException): base(message, innerException) { }

		protected EmailNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
