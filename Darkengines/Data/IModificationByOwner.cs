using Darkengines.Users.Entities;
using System;

namespace Darkengines.Data {
	public interface IModificationByOwner {
		int? ModifiedById { get; set; }
		User? ModifiedBy { get; set; }
	}
}
