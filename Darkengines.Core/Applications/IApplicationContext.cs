using Darkengines.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Applications {
	public interface IApplicationContext {
		User CurrentUser { get; }
	}
}
