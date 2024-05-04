using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Security.Cryptography;

namespace Darkengines.Authentication.Jwt {
	public class JwtAuthenticationConfiguration {
		protected AuthenticationOptions Options { get; }
		protected SymmetricSecurityKey IssuerSigningKey { get; }
		public SigningCredentials SigningCredentials { get; }
		public JsonWebTokenHandler JwtSecurityTokenHandler { get; }
		public TokenValidationParameters Parameters { get; }
		public JwtAuthenticationConfiguration(IOptions<AuthenticationOptions> options) {
			Options = options.Value;
			using (var fileStream = new FileStream(Options.RsaPrivateKeyPemPath, FileMode.Open, FileAccess.Read)) {
				using (var streamReader = new StreamReader(fileStream)) {
					var pemReader = new PemReader(streamReader);
					var pem = (AsymmetricCipherKeyPair)pemReader.ReadObject();
					var privateKeyParams = (RsaPrivateCrtKeyParameters)pem.Private;
					var privateRsa = RSA.Create(DotNetUtilities.ToRSAParameters(privateKeyParams));
					SigningCredentials = new SigningCredentials(new RsaSecurityKey(privateRsa), SecurityAlgorithms.RsaSha256);
				}
			}
			using (var fileStream = new FileStream(Options.RsaPublicKeyPemPath, FileMode.Open, FileAccess.Read)) {
				using (var streamReader = new StreamReader(fileStream)) {
					var pemReader = new PemReader(streamReader);
					var pem = (AsymmetricKeyParameter)pemReader.ReadObject();
					var publicKeyParams = (RsaKeyParameters)pem;
					var publicRsa = RSA.Create(DotNetUtilities.ToRSAParameters(publicKeyParams));
					JwtSecurityTokenHandler = new JsonWebTokenHandler();
					Parameters = new TokenValidationParameters {
						ValidateAudience = false,
						ValidIssuer = "darkengines.com",
						IssuerSigningKey = new RsaSecurityKey(publicRsa)
					};
				}
			}
		}
	}
}
