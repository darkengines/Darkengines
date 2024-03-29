﻿using Darkengines.Users.Entities;

namespace Darkengines.Data {
	public interface IActiveStateOwner {
		bool IsActive { get; set; }
		DateTimeOffset? DeactivatedOn { get; set; }
		int? DeactivatedByUserId { get; set; }
		User DeactivatedByUser { get; set; }
	}
}
