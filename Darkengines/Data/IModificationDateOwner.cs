using System;

namespace Comeet.Core.Data {
	public interface IModificationDateOwner {
		DateTimeOffset? ModifiedOn { get; set; }
	}
}
