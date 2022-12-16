using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public interface IMemberMutationInfo {
		IPropertyBase Property { get; }
		JProperty JProperty { get; }
		MemberEntry ProcessMemberEntry(MemberEntry memberEntry);
		MemberBinding BuildMemberBindingExpression(MemberEntry memberEntry, Expression instanceExpression);
	}
}
