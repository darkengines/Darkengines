using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public class NavigationPropertyTuple {
		public NavigationPropertyTuple(INavigation navigation, JProperty jProperty) {
			Navigation = navigation;
			JProperty = jProperty;
		}
		public INavigation Navigation { get; }
		public JProperty JProperty { get; }
	}
}
