using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public class ConversionArgument {
		public Type[]? GenericArguments { get; set; }
		public Type? ExpectedType { get; set; }
	}
}
