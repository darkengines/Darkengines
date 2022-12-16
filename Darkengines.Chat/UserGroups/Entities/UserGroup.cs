using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.UserGroups.Entities {
	public class UserGroup {
		public UserGroup() {
			UserUserGroups = new Collection<UserUserGroup>();
		}
		public int Id { get; set; }
		public string DisplayName { get; set; }
		public virtual ICollection<UserUserGroup> UserUserGroups { get; }
	}
}
