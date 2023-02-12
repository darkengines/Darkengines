using Darkengines.Users.Entities;
using System;

namespace Comeet.Core.Data {
	public interface IModificationByOwner {
		int? ModifiedById { get; set; }
		User? ModifiedBy { get; set; }
	}
}
