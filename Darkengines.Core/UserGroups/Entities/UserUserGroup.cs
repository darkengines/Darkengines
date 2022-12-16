using Darkengines.Core.Users.Entities;

namespace Darkengines.Core.UserGroups.Entities {
	public class UserUserGroup {
		public int? UserId { get; set; }
		public virtual User User { get; set; }
		public int UserGroupId { get; set; }
		public virtual UserGroup UserGroup { get; set; }
		public int Index { get; set; }
	}
}