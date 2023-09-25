using Darkengines.Authentication;
using Darkengines.Expressions;
using Darkengines.Data;
using Darkengines.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Apis.FluentApi {
    public class ConverterScopeProvider : IConverterScopeProvider {
        public ConverterScopeProvider(
            ApplicationDbContext applicationDbContext,
            Authentication.Authentication authentication,
            Mutation mutation,
            ModelProvider modelProvider
        ) {
            var queries = applicationDbContext.Model.GetEntityTypes().Select(entityType => {
                return new KeyValuePair<string, Expression>(entityType.ShortName(), Expression.Constant(applicationDbContext.GetQuery(entityType.ClrType)));
            });

            var identifiers = new Dictionary<string, Expression>() {
                { nameof(ModelProvider), Expression.Constant(modelProvider) },
                { nameof(Authentication), Expression.Constant(authentication) },
                { nameof(Mutation), Expression.Constant(mutation) }
            };
            foreach (var query in queries) identifiers.Add(query.Key, query.Value);

            Identifiers = identifiers;
        }
        protected IDictionary<string, Expression> Identifiers { get; }
        public IDictionary<string, Expression> GetIdentifiers() {
            return Identifiers;
        }
    }
}
