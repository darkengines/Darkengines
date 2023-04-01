using System;

namespace Darkengines.Data {
	public interface IModificationDateOwner {
		DateTimeOffset? ModifiedOn { get; set; }
	}
}
