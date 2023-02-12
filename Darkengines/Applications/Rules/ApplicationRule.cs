using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Applications.Rules
{
	public class ApplicationRule: TypeRuleMap<Entities.Application, IApplicationContext> {
		public ApplicationRule() {
			Expose(application => application.Id!).WithOperation(Operation.Read);
			Expose(application => application.Name!).WithOperation(Operation.Read);
			Expose(application => application.DisplayName!).WithOperation(Operation.Read);
			Expose(application => application.UserApplications!).WithOperation(Operation.Read);
		}
	}
}
