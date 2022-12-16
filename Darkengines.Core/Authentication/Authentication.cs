using Darkengines.Core.Authentication.Jwt;
using Darkengines.Expressions.Converters;
using Darkengines.Expressions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using Darkengines.Core.Data;
using Microsoft.EntityFrameworkCore;
using Darkengines.Core.Users.Entities;
using Darkengines.Core.Applications;
using Comeet.Core.Common;

namespace Darkengines.Core.Authentication {
	public class Authentication {
		protected JwtAuthenticationConfiguration JwtAuthenticationConfiguration { get; }
		protected IConfiguration Configuration { get; }
		protected JsonSerializer JsonSerializer { get; }
		protected JsonSerializer JwtJsonSerializer { get; }
		protected IHttpContextAccessor HttpContextAccessor { get; }
		protected ILogger Logger { get; }
		protected ApplicationDbContext ApplicationDbContext { get; }
		protected AuthenticationOptions AuthenticationOptions { get; }
		protected IIdentityProvider IdentityProvider { get; }

		public Authentication(
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration,
			IConfiguration configuration,
			JsonSerializer jsonSerializer,
			ILogger<Authentication> logger,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AuthenticationOptions> options,
			ApplicationDbContext applicationDbContext,
			IIdentityProvider identityProvider
		) {
			ApplicationDbContext = applicationDbContext;
			IdentityProvider = identityProvider;
			Logger = logger;
			HttpContextAccessor = httpContextAccessor;
			AuthenticationOptions = options.Value;
			JwtAuthenticationConfiguration = jwtAuthenticationConfiguration;
			Configuration = configuration;
			JsonSerializer = jsonSerializer;

			JwtJsonSerializer = new JsonSerializer();
			JwtJsonSerializer.ContractResolver = new JwtPropertyContractResolver();
			JwtJsonSerializer.TypeNameHandling = TypeNameHandling.Objects;
			JwtJsonSerializer.Formatting = Formatting.Indented;

			JsonExtensions.Serializer = @object => {
				using (var stringWriter = new StringWriter()) {
					jsonSerializer.Serialize(stringWriter, @object);
					return stringWriter.ToString();
				}
			};
			JsonExtensions.Deserializer = (text, type) => {
				using (var stringReader = new StringReader(text)) {
					return JsonSerializer.Deserialize(stringReader, type);
				}
			};
		}
		public async Task<string> Create(string email, string password) {
			var hashedEmailAddress = ToLowerInvariantSHA256(email);
			var hashedPassword = ToSHA256(password);

			var userEmailAddress = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.IsVerified
					&& userEmailAddress.HashedEmailAddress == ToLowerInvariantSHA256(email)
				);
			if (userEmailAddress != null) throw new EmailAlreadyInUseException();
			var user = new User() {
				HashedPassword = hashedPassword,
			};
			user.UserEmailAddresses.Add(new UserEmailAddress {
				EmailAddress = email,
				HashedEmailAddress = hashedEmailAddress
			});

			await ApplicationDbContext.Users.AddAsync(user);
			await ApplicationDbContext.SaveChangesAsync();
			return await BuildToken(user);
		}
		public async Task<string> Login(string email, string password) {
			var userEmailAddress = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.IsVerified
					&& userEmailAddress.HashedEmailAddress == ToLowerInvariantSHA256(email)
				);
			if (userEmailAddress != null) {
				byte[] hashedPassword = ToSHA256(password);
				if (userEmailAddress.User.HashedPassword.SequenceEqual(hashedPassword)) return await BuildToken(userEmailAddress.User);
			}
			throw new InvalidCredentialException();
		}
		public async Task DeleteAsync() {
			User user = await ApplicationDbContext.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.Id == IdentityProvider.GetIdentity().User.Id);
			user.IsActive = false;
			ApplicationDbContext.SaveChanges();
		}
		public async Task<string> BuildToken(User user) {
			var unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0));
			var now = DateTimeOffset.UtcNow;
			var payload = new JwtPayload() {
				{ "sub", user.Id },
				{ "iss", JwtAuthenticationConfiguration.Parameters.ValidIssuer },
				{ "iat", Math.Floor((now - unixEpoch).TotalSeconds) },
				{ "nbf", Math.Floor((now - unixEpoch).TotalSeconds) },
				{ "exp", Math.Floor((now.AddYears(1) - unixEpoch).TotalSeconds) },
				{ "user", JObject.FromObject(user,  JwtJsonSerializer) }
			};
			var jwt = new JwtSecurityToken(JwtAuthenticationConfiguration.JwtHeader, payload);
			var token = JwtAuthenticationConfiguration.JwtSecurityTokenHandler.WriteToken(jwt);
			user.LastIpAddress = HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
			ApplicationDbContext.Update(user);
			await ApplicationDbContext.SaveChangesAsync();
			return token;
		}

		protected async Task<UserEmailAddress> FetchUserEmailAddress(string email) {
			var userEmail = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.IsVerified
					&& userEmailAddress.HashedEmailAddress == ToLowerInvariantSHA256(email)
				);
			return userEmail;
		}

		public async Task RequestPasswordReset(string email, string resetPasswordUrl) {
			throw new NotImplementedException();
			var userEmail = await FetchUserEmailAddress(email);
			if (userEmail == null) throw new EmailNotFoundException();

			await ApplicationDbContext.SaveChangesAsync();
		}

		public async Task ResetPassword(Guid guid, string password) {
			throw new NotImplementedException();
			var userPasswordResetRequest = ApplicationDbContext.UserPasswordResetRequests.FirstOrDefault(userPasswordResetRequest =>
				userPasswordResetRequest.Guid == guid
				&& userPasswordResetRequest.CreatedOn.Value.AddHours(1) > DateTimeOffset.Now
			);
			if (userPasswordResetRequest == null) throw new EmailNotFoundException();

			await ApplicationDbContext.SaveChangesAsync();
		}

		public async Task ChangePassword(string email, string oldPassword, string newPassword) {
			if (string.IsNullOrWhiteSpace(oldPassword) || String.IsNullOrWhiteSpace(newPassword)) throw new NotFoundException();
			var hashedOldPassword = ToSHA256(oldPassword);
			var hashedNewPassword = ToSHA256(newPassword);
			var userEmail = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.IsVerified
					&& userEmailAddress.HashedEmailAddress == ToLowerInvariantSHA256(email) && userEmailAddress.User.HashedPassword.SequenceEqual(hashedOldPassword)
				);
			if (userEmail == null) throw new NotFoundException();
		}

		public static byte[] ToLowerInvariantSHA256(string @string) {
			return ToSHA256(@string.ToLowerInvariant());
		}
		public static byte[] ToSHA256(string @string) {
			byte[] hashedValue = null;
			using (var sha256 = SHA256.Create()) {
				hashedValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(@string));
			}
			return hashedValue;
		}

		public bool IsValidEmail(string email) {

			return CheckEntryValidity(email, @"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$");
		}

		public bool IsPasswordValid(string pswd) {
			return CheckEntryValidity(pswd, @"^((?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,})$");
		}


		private bool CheckEntryValidity(string value, string pattern) {
			if (string.IsNullOrWhiteSpace(value))
				return false;
			try {
				return Regex.IsMatch(value, pattern,
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			} catch (RegexMatchTimeoutException) {
				return false;
			}

		}
	}
}