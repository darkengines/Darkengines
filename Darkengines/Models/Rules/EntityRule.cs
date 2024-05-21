using Darkengines.Applications;
using Darkengines.Models.Entities;
using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Models.Rules {
	public class EntityRule : TypeRuleMap<Entity, IApplicationContext> {
		public EntityRule() {
			WithOperation(Operation.Manage);
			Expose(entity => entity.Name).WithOperation(Operation.Read);
			Expose(entity => entity.DisplayName).WithOperation(Operation.Read);
			Expose(entity => entity.Description).WithOperation(Operation.Read);
			Expose(entity => entity.ModelName).WithOperation(Operation.Read);
			Expose(entity => entity.Model).WithOperation(Operation.Read);
		}
	}
}
