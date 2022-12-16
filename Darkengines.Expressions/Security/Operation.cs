using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	[Flags]
	public enum Operation {
		None = 0,
		Read = 1,
		Edit = 2,
		Create = 4,
		Write = Edit | Create,
		ReadWrite = Read | Write,
		Delete = 8,
		Manage = ReadWrite | Delete,
		ReadBackExpression = 16
	}
}
