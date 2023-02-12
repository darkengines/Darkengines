using Darkengines.Applications;
using Darkengines.Data;
using Darkengines.UserGroups.Entities;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Queryable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Test {
	[TestClass]
	public class SqlServerTest {
		protected IServiceProvider ServiceProvider { get; }
		public SqlServerTest() {
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddTestApplicationContext().AddDarkengines();
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}
		[TestMethod]
		public async Task TestSqlServer() {
			var testApplicationContext = ServiceProvider.GetRequiredService<TestApplicationContext>();
			await testApplicationContext.ApplicationInitializer.Initialize();

			testApplicationContext.ApplicationDbContext.ChangeTracker.Clear();
			//var query = applicationDbContext.Users.WithProjection(user => new User() { DisplayName = user.Id == 0 ? user.DisplayName : null });
			//var query = applicationDbContext.Users.WithProjection(user => new User() { Id = user.Id, DisplayName = user.Id == 0 ? user.DisplayName : null }).Include(user => user.UserUserGroups);
			//var query = applicationDbContext.Users.Select(user => new User() { DisplayName = user.Id == 0 ? user.DisplayName : null }).AsQueryable().Include(user => user.UserUserGroups);
			//var query = applicationDbContext.Users.Include(user => user.UserUserGroups).ThenInclude(uug => uug.UserGroup.Select(uug => new UserGroup() {
			//var query = applicationDbContext.Users.Join(applicationDbContext.UserUserGroups, u => u.Id, uug => uug.UserId, (u, uug) => new UserUserGroup() { UserId = uug.UserId, UserGroupId = uug.UserGroupId}).Include(uug => uug.UserGroup);
			var query = testApplicationContext.ApplicationDbContext.Users;
			var queryString = query.ToQueryString();
			var userUserGroups = query.ToArray();
		}
	}
}
