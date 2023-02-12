﻿using Darkengines.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Test {
	public static class UserExtensions {
		public static string? GetDisplayName(this User user) { return user?.UserProfile?.DisplayName; }
	}
}
