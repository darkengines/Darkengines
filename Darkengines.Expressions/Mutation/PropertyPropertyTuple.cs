using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public class PropertyPropertyTuple {
		public PropertyPropertyTuple(IProperty property, JProperty jPropperty) {
			Property = property;
			JProperty = jPropperty;
		}
		public IProperty Property { get; }
		public JProperty JProperty { get; }
	}
}
