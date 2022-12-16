using Darkengines.Expressions.Application.Users.Entities;

namespace Darkengines.Expressions.Application.UserGroups.Entities {
	public class UserUserGroup {
		public int? UserId { get; set; }
		public virtual User User { get; set; }
		public int UserGroupId { get; set; }
		public virtual UserGroup UserGroup { get; set; }
		public int Index { get; set; }
	}
}