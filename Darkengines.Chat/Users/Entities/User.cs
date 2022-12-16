using Darkengines.Expressions.Application.UserGroups.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Users.Entities {
	public class User {
		public User() {
			UserUserGroups = new Collection<UserUserGroup>();
		}
		public int? Id { get; set; }
		public string? DisplayName { get; set; }
		public string? Firstname { get; set; }
		public string? Lastname { get; set; }
		public string? EmailAddress { get; set; }
		public string? HashedEmailAddress { get; set; }
		public byte[] HashedPassword { get; set; }
		public virtual ICollection<UserUserGroup> UserUserGroups { get; }
		public int? ApplicationId { get; set; }
		public virtual Applications.Entities.Application? Application { get; set; }
	}
}
