using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions {
	public static class BuiltInDotNetTypeMap {
		public static Type? GetClrType(string typeName) {
			var type = default(Type);
			if (KeywordDotNetTypeMap.TryGetValue(typeName, out type)) return type;
			return Type.GetType(typeName, false);
		}
		public static Dictionary<string, Type> KeywordDotNetTypeMap = new Dictionary<string, Type>() {
			{"bool", typeof(bool) },
			{"byte", typeof(byte) },
			{"sbyte", typeof(sbyte) },
			{"char", typeof(char) },
			{"decimal", typeof(decimal) },
			{"double", typeof(double) },
			{"float", typeof(float) },
			{"int", typeof(int) },
			{"uint", typeof(uint) },
			{"nint", typeof(nint) },
			{"nuint", typeof(nuint) },
			{"long", typeof(long) },
			{"ulong", typeof(ulong) },
			{"short", typeof(short) },
			{"ushort", typeof(ushort) },
			{"string", typeof(string) },
		};
	}
}
