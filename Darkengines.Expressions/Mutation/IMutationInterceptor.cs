using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Mutation {
	public interface IMutationInterceptor {
		public void PopulatingEntity(JObject jObject, EntityEntry entityEntry);
	}
}
