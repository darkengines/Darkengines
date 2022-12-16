using Darkengines.Expressions.Application.Users.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Applications.Entities {
	public class Application {
		public Application() {
			Users = new Collection<User>();
		}
		public int? Id { get; set; }
		public string? Name { get; set; }
		public string? DisplayName { get; set; }
		public ICollection<User> Users { get; }
	}
}
