namespace Darkengines.Identity {
	public class ResourceOwnerPasswordCredentialsRequest {
		public string GrantType { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public ICollection<string> Scope { get; set; }
		public string Format { get; set; }
	}
}
