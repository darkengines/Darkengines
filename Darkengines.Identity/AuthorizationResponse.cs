namespace Darkengines.Identity {
	public class AuthorizationResponse {
		public string ResponseType { get; set; }
		public string Code { get; set; }
		public string State { get; set; }
		public Uri RedirectUri { get; set; }
		public ICollection<string> RequestedScopes { get; set; }
		public string GrantType { get; set; }
		public string Nonce { get; set; }
		public string Error { get; set; }
		public Uri ErrorUri { get; set; }
		public string ErrorDescription { get; set; }
	}
}
