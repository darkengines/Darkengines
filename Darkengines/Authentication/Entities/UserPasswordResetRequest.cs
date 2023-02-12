using Comeet.Core.Data;
using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Authentication.Entities {
	public class UserPasswordResetRequest: IMonitored {
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public Guid Guid { get; set; }
		public int? CreatedById { get; set; }
		public virtual User? CreatedBy { get; set; }
		public DateTimeOffset? CreatedOn { get; set; }
		public int? ModifiedById { get; set; }
		public virtual User? ModifiedBy { get; set; }
		public DateTimeOffset? ModifiedOn { get; set; }
	}
}
