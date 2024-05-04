using Darkengines.Users.Entities;

namespace Darkengines.Users.Templates.Models {
    public class UserPasswordResetTokenModel {
		public required Guid Token { get; set; }
		public required Uri PasswordResetUri { get; set; }
	}
}