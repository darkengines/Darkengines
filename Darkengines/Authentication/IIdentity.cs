using Darkengines.Users.Entities;

namespace Darkengines.Authentication {
	public interface IIdentity {
		User User { get; }
	}
}
