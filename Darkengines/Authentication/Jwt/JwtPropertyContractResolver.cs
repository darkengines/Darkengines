using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Darkengines.Authentication.Jwt {
	public class JwtPropertyContractResolver : CamelCasePropertyNamesContractResolver {
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			var property = base.CreateProperty(member, memberSerialization);
			property.ShouldSerialize = (@object) => member.GetCustomAttribute<JwtPropertyAttribute>(true) != null;
			return property;
		}
	}
}