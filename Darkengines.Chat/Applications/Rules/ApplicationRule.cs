using Darkengines.Expressions.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Application.Applications.Rules {
	public class ApplicationRule: TypeRuleMap<Entities.Application, ApplicationContext> {
		public ApplicationRule() {
			Property(application => application.Id!).WithOperation(Operation.Read);
			Property(application => application.Name!).WithOperation(Operation.Read);
			Property(application => application.DisplayName!).WithOperation(Operation.Read);
			Property(application => application.Users!).WithOperation(Operation.Read);
		}
	}
}
