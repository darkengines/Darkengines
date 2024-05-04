namespace Darkengines.Authentication.Jwt {
	public class AuthenticationOptions {
		public string Issuer { get; set; }
		public string RsaPrivateKeyPemPath { get; set; }
		public string RsaPublicKeyPemPath { get; set; }
        public Uri[] AllowedClientUris { get; set; }
    }
}
