using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Darkengines.Expressions.Mutation {
	public class MutationContext {
		public PermissionEntityTypeBuilder PermissionEntityTypeBuilder { get; }
		public IEnumerable<IRuleMap> RuleMaps { get; }
		public IEnumerable<IMutationInterceptor> MutationInterceptors { get; }
		public object SecurityContext { get; }
		public JsonSerializer JsonSerializer { get; }
		public IDictionary<string, JObject> JObjectReferenceMapping { get; }
		public IDictionary<JObject, EntityMutationInfo> JObjectEntityMutationInfoMapping { get; }
		public DbContext DbContext { get; }
		public MutationContext(
			PermissionEntityTypeBuilder permissionEntityTypeBuilder,
			IEnumerable<IRuleMap> ruleMaps,
			object securityContext,
			JsonSerializer jsonSerializer,
			DbContext dbContext,
			IEnumerable<IMutationInterceptor> mutationInterceptors
		) {
			MutationInterceptors = mutationInterceptors;
			JObjectReferenceMapping = new Dictionary<string, JObject>();
			JObjectEntityMutationInfoMapping = new Dictionary<JObject, EntityMutationInfo>();
			PermissionEntityTypeBuilder = permissionEntityTypeBuilder;
			RuleMaps = ruleMaps;
			SecurityContext = securityContext;
			JsonSerializer = jsonSerializer;
			DbContext = dbContext;
		}
	}
}
