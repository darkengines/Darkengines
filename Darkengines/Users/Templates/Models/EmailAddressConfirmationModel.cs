using Darkengines.Users.Entities;

namespace Darkengines.Users.Templates.Models {
    public class EmailAddressConfirmationModel {
        public required UserEmailAddress UserEmailAddress { get; set; }
		public required Uri EmailAddressConfirmationUri { get; set; }
	}
}