using Darkengines.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Applications.Entities {
	public class UserApplication {
		public int UserId { get; set; }
		public User User { get; set; }
		public int ApplicationId { get; set; }
		public Application Application { get; set; }
	}
}
