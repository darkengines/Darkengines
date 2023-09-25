using System.Linq.Expressions;

namespace Darkengines.Apis.FluentApi {
    public class FluentApiContext {
        public IDictionary<string, Expression> Identifiers { get; } 
        public FluentApiContext() {
            Identifiers = new Dictionary<string, Expression>();
        }
    }
}