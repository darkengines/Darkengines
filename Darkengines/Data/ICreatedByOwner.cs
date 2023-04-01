using Darkengines.Users.Entities;

namespace Darkengines.Data {
	public interface ICreatedByOwner {
		int? CreatedById { get; set; }
		User? CreatedBy { get; set; }
	}
}
