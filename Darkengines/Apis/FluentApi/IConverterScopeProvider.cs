using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Apis.FluentApi {
    public interface IConverterScopeProvider {
        public IDictionary<string, Expression> GetIdentifiers();
    }
}
