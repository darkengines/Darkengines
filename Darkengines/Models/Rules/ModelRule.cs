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

namespace Darkengines.Models.Rules
{
    public class ModelRule : TypeRuleMap<Model, IApplicationContext>
    {
        public ModelRule()
        {
            WithOperation(Operation.Manage);
            Expose(model => model.Name);
            Expose(model => model.DisplayName);
            Expose(model => model.Description);
        }
    }
}
