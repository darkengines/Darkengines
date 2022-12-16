using Darkengines.Core.Users.Entities;

namespace Darkengines.Core.Authentication {
	public interface IIdentity {
		User User { get; }
	}
}
