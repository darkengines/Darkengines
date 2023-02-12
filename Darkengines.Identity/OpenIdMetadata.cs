namespace Darkengines.Identity {
	public class OpenIdMetadata {
		public string Issuer { get; set; }
		public string AuthorizationEndpoint { get; set; }
		public string TokenEndpoint { get; set; }
		public ICollection<string> TokenEndpointAuthMethodsSupported { get; set; }
		public ICollection<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; }
		public string UserinfoEndpoint { get; set; }
		public string CheckSessionIframe { get; set; }
		public string EndSessionEndpoint { get; set; }
		public string JwksUri { get; set; }
		public string RegistrationEndpoint { get; set; }
		public ICollection<string> ScopesSupported { get; set; }
		public ICollection<string> ResponseTypesSupported { get; set; }
		public ICollection<string> AcrValuesSupported { get; set; }
		public ICollection<string> SubjectTypesSupported { get; set; }
		public ICollection<string> UserinfoSigningAlgValuesSupported { get; set; }
		public ICollection<string> UserinfoEncryptionAlgValuesSupported { get; set; }
		public ICollection<string> UserinfoEncryptionEncValuesSupported { get; set; }
		public ICollection<string> IdTokenSigningAlgValuesSupported { get; set; }
		public ICollection<string> IdTokenEncryptionAlgValuesSupported { get; set; }
		public ICollection<string> IdTokenEncryptionEncValuesSupported { get; set; }
		public ICollection<string> RequestObjectSigningAlgValuesSupported { get; set; }
		public ICollection<string> DisplayValuesSupported { get; set; }
		public ICollection<string> ClaimTypesSupported { get; set; }
		public ICollection<string> ClaimsSupported { get; set; }
		public bool ClaimsParameterSupported { get; set; }
		public string ServiceDocumentation { get; set; }
		public ICollection<string> UiLocalesSupported { get; set; }
	}
}
