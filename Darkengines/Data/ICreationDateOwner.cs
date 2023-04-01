using System;

namespace Darkengines.Data {
	public interface ICreationDateOwner {
		DateTimeOffset? CreatedOn { get; set; }
	}
}
