using Comeet.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Users.Entities {
	public class UserEmailAddress : IMonitored {
		public int UserId { get; set; }
		public User User { get; set; }
		public string? EmailAddress { get; set; }
		public byte[]? HashedEmailAddress { get; set; }
		public Guid? Guid { get; set; }
		public bool IsVerified { get; set; }
		public int? CreatedById { get; set; }
		public User? CreatedBy { get; set; }
		public DateTimeOffset? CreatedOn { get; set; }
		public int? ModifiedById { get; set; }
		public User? ModifiedBy { get; set; }
		public DateTimeOffset? ModifiedOn { get; set; }
	}
}
