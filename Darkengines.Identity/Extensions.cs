using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Identity {
	public static class Extensions {
		public static IMvcBuilder AddIdentity(this IMvcBuilder mvcBuilder) {
			return mvcBuilder.AddApplicationPart(Assembly.GetExecutingAssembly());
		}
	}
}
