using Darkengines.Core.Users.Entities;
using System.Collections.ObjectModel;

namespace Darkengines.Core.Applications.Entities {
	public class Application {
		public Application() {
			UserApplications = new Collection<UserApplication>();
		}
		public int? Id { get; set; }
		public string? Name { get; set; }
		public string? DisplayName { get; set; }
		public ICollection<UserApplication> UserApplications { get; }
	}
}
