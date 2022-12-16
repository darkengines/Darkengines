using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Expressions.Security {
	public class PermissionEntity {
		public static PropertyInfo SelfPermissionPropertyInfo = ExpressionHelper.ExtractPropertyInfo<PermissionEntity>(permissionEntity => permissionEntity.SelfPermission!);
		public bool SelfPermission { get; set; }
	}
}
