using Darkengines.Core.Users.Entities;

namespace Comeet.Core.Data {
	public interface ICreatedByOwner {
		int? CreatedById { get; set; }
		User? CreatedBy { get; set; }
	}
}
