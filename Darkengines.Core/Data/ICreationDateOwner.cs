using System;

namespace Comeet.Core.Data {
	public interface ICreationDateOwner {
		DateTimeOffset? CreatedOn { get; set; }
	}
}
