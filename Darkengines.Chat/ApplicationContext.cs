using Darkengines.Expressions.Application.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application {
	public class ApplicationContext {
		public User CurrentUser { get; }
		public bool IsAdmin { get; }
		public ApplicationContext(User currentUser, bool isAdmin) {
			CurrentUser = currentUser;
			IsAdmin = isAdmin;
		}		
	}
}
