using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public abstract class NavigationMutationInfo : IMemberMutationInfo {
		public INavigation Navigation { get; }
		public JProperty JProperty { get; }
		public IPropertyBase Property { get { return Navigation; } }
		public EntityMutationInfo EntityMutationInfo { get; }
		public abstract NavigationEntry ProcessNavigationEntry(NavigationEntry navigationEntry);
		MemberEntry IMemberMutationInfo.ProcessMemberEntry(MemberEntry memberEntry) {
			return ProcessNavigationEntry((NavigationEntry)memberEntry);
		}

		public abstract MemberBinding BuildMemberBindingExpression(MemberEntry memberEntry, Expression instanceExpression);
		public NavigationMutationInfo(EntityMutationInfo entityMutationInfo, INavigation navigation, JProperty jProperty) {
			EntityMutationInfo = entityMutationInfo;
			Navigation = navigation;
			JProperty = jProperty;
		}

	}
}
