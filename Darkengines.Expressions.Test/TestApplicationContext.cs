using Darkengines.Core.Applications;
using Darkengines.Core.Data;
using Darkengines.Core.Users.Entities;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Test {
	public class TestApplicationContext {
		public ApplicationInitializer ApplicationInitializer { get; }
		public IEnumerable<IRuleMap> RuleMaps { get; }
		public ApplicationDbContext ApplicationDbContext { get; }
		public IModel Model { get; }

		public TestApplicationContext(ApplicationInitializer applicationInitializer, IEnumerable<IRuleMap> ruleMaps, ApplicationDbContext applicationDbContext, IModel model) {
			ApplicationInitializer = applicationInitializer;
			RuleMaps = ruleMaps;
			ApplicationDbContext = applicationDbContext;
			Model = model;
		}
	}
}
