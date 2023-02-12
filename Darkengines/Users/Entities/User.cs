using Comeet.Core.Data;
using Darkengines.Authentication.Entities;
using Darkengines.Authentication.Jwt;
using Darkengines.UserGroups.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Users.Entities {
	public class User : IMonitored, IActiveStateOwner {
		public User() {
			UserUserGroups = new Collection<UserUserGroup>();
			UserEmailAddresses = new Collection<UserEmailAddress>();
			UserPasswordResetRequests = new Collection<UserPasswordResetRequest>();
		}
		[JwtProperty]
		public int Id { get; set; }
		public byte[]? HashedPassword { get; set; }
		public UserProfile? UserProfile { get; set; }
		public UserSettings? UserSettings { get; set; }
		public virtual ICollection<UserEmailAddress> UserEmailAddresses { get; set; }
		public virtual ICollection<UserUserGroup> UserUserGroups { get; }
		public virtual ICollection<UserPasswordResetRequest> UserPasswordResetRequests { get; }
		public string? LastIpAddress { get; set; }
		public bool IsActive { get; set; }
		public DateTimeOffset? DeactivatedOn { get; set; }
		public int? DeactivatedByUserId { get; set; }
		public User? DeactivatedByUser { get; set; }
		public int? CreatedById { get; set; }
		public User? CreatedBy { get; set; }
		public DateTimeOffset? CreatedOn { get; set; }
		public int? ModifiedById { get; set; }
		public User? ModifiedBy { get; set; }
		public DateTimeOffset? ModifiedOn { get; set; }
	}
}
