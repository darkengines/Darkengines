using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public static class DictionaryExtensions {
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory) {
			var value = default(TValue);
			if (!dictionary.TryGetValue(key, out value)) {
				dictionary[key] = value = valueFactory();
			}
			return value;
		}
	}
}
