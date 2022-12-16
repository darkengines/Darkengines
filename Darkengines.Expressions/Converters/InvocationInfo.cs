using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Converters {
	public class InvocationInfo {
		public MethodInfo MethodInfo { get; set; }
		public InvocationArgumentInfo? MemberInfo { get; set; }
		public IEnumerable<InvocationArgumentInfo> ArgumentInfos { get; }
		public ParameterInfo[] ParameterInfos { get; }
		public InvocationInfo(
			MethodInfo methodInfo,
			ParameterInfo[] parameterInfos,
			InvocationArgumentInfo? memberInfo,
			IEnumerable<InvocationArgumentInfo> argumentInfos
		) {
			MethodInfo = methodInfo;
			ParameterInfos = parameterInfos;
			MemberInfo = memberInfo;
			ArgumentInfos = argumentInfos;
		}
	}
}
