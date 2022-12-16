using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public class ConverterScope {
		public ConverterScope(ConverterScope? parent = null) {
			Parent = parent;
		}
		public Expression? ResolveIdentifier(string identifier) {
			if (Identifiers.TryGetValue(identifier, out var expression)) {
				return expression;
			}
			return Parent?.ResolveIdentifier(identifier);
		}
		public ConverterScope? Parent { get; set; }
		public IDictionary<string, Expression> Identifiers { get; set; } = new Dictionary<string, Expression>();
	}
}
