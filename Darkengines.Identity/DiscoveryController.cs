using Microsoft.AspNetCore.Mvc;

namespace Darkengines.Identity {
	public class DiscoveryController : Controller {
		[HttpGet("~/.well-known/openid-configuration")]
		public OpenIdMetadata GetOpenIdMetadata() {
			var metadata = new OpenIdMetadata {
				Issuer = "https://localhost:7275",
				AuthorizationEndpoint = "https://localhost:7275/Home/Authorize",
				TokenEndpoint = "https://localhost:7275/Home/Token",
				TokenEndpointAuthMethodsSupported = new string[] { "client_secret_basic", "private_key_jwt" },
				TokenEndpointAuthSigningAlgValuesSupported = new string[] { "RS256", "ES256" },

				AcrValuesSupported = new string[] { "urn:mace:incommon:iap:silver", "urn:mace:incommon:iap:bronze" },
				ResponseTypesSupported = new string[] { "code", "code id_token", "id_token", "token id_token" },
				SubjectTypesSupported = new string[] { "public", "pairwise" },

				UserinfoEncryptionEncValuesSupported = new string[] { "A128CBC-HS256", "A128GCM" },
				IdTokenSigningAlgValuesSupported = new string[] { "RS256", "ES256", "HS256" },
				IdTokenEncryptionAlgValuesSupported = new string[] { "RSA1_5", "A128KW" },
				IdTokenEncryptionEncValuesSupported = new string[] { "A128CBC-HS256", "A128GCM" },
				RequestObjectSigningAlgValuesSupported = new string[] { "none", "RS256", "ES256" },
				DisplayValuesSupported = new string[] { "page", "popup" },
				ClaimTypesSupported = new string[] { "normal", "distributed" },

				ScopesSupported = new string[] { "openid", "profile", "email", "address", "phone", "offline_access" },
				ClaimsSupported = new string[] { "sub", "iss", "auth_time", "acr", "name", "given_name",
					"family_name", "nickname", "profile", "picture", "website", "email", "email_verified",
					"locale", "zoneinfo" },
				ClaimsParameterSupported = true,
				ServiceDocumentation = "https://localhost:7275/connect/service_documentation.html",
				UiLocalesSupported = new string[] { "en-US", "en-GB", "en-CA", "fr-FR", "fr-CA" }

			};
			return metadata;
		}
	}
}
