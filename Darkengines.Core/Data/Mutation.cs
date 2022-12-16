using Darkengines.Core.Applications;
using Darkengines.Core.Security;
using Darkengines.Expressions;
using Darkengines.Expressions.Mutation;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Core.Data {
	public class Mutation {
		protected JsonSerializer JsonSerializer { get; }
		protected IModel Model { get; }
		protected PermissionEntityTypeBuilder PermissionEntityTypeBuilder { get; }
		protected IEnumerable<IRuleMap> RuleMaps { get; }
		protected ApplicationDbContext ApplicationDbContext { get; }
		protected IApplicationContext ApplicationContext { get; }
		public Mutation(
			JsonSerializer jsonSerializer,
			IModel model,
			PermissionEntityTypeBuilder permissionEntityTypeBuilder,
			IEnumerable<IRuleMap> ruleMaps,
			ApplicationDbContext applicationDbContext,
			IApplicationContext applicationContext
		) {
			JsonSerializer = jsonSerializer;
			Model = model;
			PermissionEntityTypeBuilder = permissionEntityTypeBuilder;
			RuleMaps = ruleMaps;
			ApplicationDbContext = applicationDbContext;
			ApplicationContext = applicationContext;
		}
		public async Task<object> Mutate(string type, JObject jObject) {
			var entityType = Model.FindEntityType(type);
			var mutationContext = new MutationContext(PermissionEntityTypeBuilder, RuleMaps, ApplicationContext, JsonSerializer, ApplicationDbContext);
			var entityMutationInfo = new EntityMutationInfo(entityType, jObject, mutationContext);
			var entry = entityMutationInfo.GetEntry();
			var entity = entry.Entity;
			var query = EntityMutationInfo.BuildFindEntityQuery(ApplicationDbContext, entityType, entity);
			var entityParameterExpression = Expression.Parameter(entityType.ClrType);
			var permissionExpression = entityMutationInfo.BuildEntityPermissionExpression(entry, entityParameterExpression);

			var selectLambdaExpression = Expression.Lambda(permissionExpression, entityParameterExpression);
			var selectCallExpression = (IQueryable)EntityMutationInfo.SelectMethodInfo.MakeGenericMethod(entityType.ClrType, entityMutationInfo.PermissionEntityType).Invoke(null, new object[] { query, selectLambdaExpression });

			var permissionEntity = selectCallExpression.GetEnumerator().Current;

			await ApplicationDbContext.SaveChangesAsync();
			return entityMutationInfo.FilteredJObject;
		}
	}
}
