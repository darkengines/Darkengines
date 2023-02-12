using System.Security.Claims;

namespace Darkengines.Identity {
	public class AuthorizationCode {
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public Uri RedirectUri { get; set; }
		public DateTimeOffset CreationTime { get; set; }
		public bool IsOpenId { get; set; }
		public ICollection<string> RequestedScopes { get; set; }
		public ClaimsPrincipal Subject { get; set; }
		public string Nonce { get; set; }
	}
}
