using Darkengines.Applications;
using Darkengines.Applications.Entities;
using Darkengines.Authentication;
using Darkengines.Data;
using Darkengines.UserGroups.Entities;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Mutation;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Test {
	[TestClass]
	public class MutationTest {
		protected IServiceProvider ServiceProvider { get; }
		public MutationTest() {
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddTestApplicationContext();
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}
		[TestMethod]
		public async Task TestMutation() {
			var testApplicationContext = ServiceProvider.GetRequiredService<TestApplicationContext>();
			await testApplicationContext.ApplicationInitializer.Initialize();

			var model = new UserUserGroup() {
				UserId = 2,
				UserGroupId = 2,
				Index = 69
			};

			testApplicationContext.ApplicationDbContext.ChangeTracker.Clear();
			var securityContext = testApplicationContext;
			var entityPermissionTypeBuilder = new PermissionEntityTypeBuilder(testApplicationContext.ApplicationDbContext.Model);
			var jsonSerializer = new JsonSerializer();
			jsonSerializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
			jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			jsonSerializer.NullValueHandling = NullValueHandling.Ignore;

			var serializedUser = JObject.FromObject(model, jsonSerializer);
			var userEntityType = testApplicationContext.ApplicationDbContext.Model.GetEntityTypes().First(entityType => entityType.ClrType == model.GetType());
			var mutationContext = new MutationContext(entityPermissionTypeBuilder, testApplicationContext.RuleMaps, securityContext, jsonSerializer, testApplicationContext.ApplicationDbContext, null);
			var entityMutationInfo = new EntityMutationInfo(userEntityType, serializedUser, mutationContext);
			var entry = entityMutationInfo.GetEntry();
			await testApplicationContext.ApplicationDbContext.SaveChangesAsync();
		}
	}
}
