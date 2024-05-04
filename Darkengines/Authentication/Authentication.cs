using Darkengines.Authentication.Jwt;
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
using Darkengines.Data;
using Microsoft.EntityFrameworkCore;
using Darkengines.Users.Entities;
using Darkengines.Applications;
using Darkengines.Common;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using Darkengines.Mailing;
using RazorLight;
using Darkengines.Users.Templates.Models;
using Darkengines.Users.Templates.Views;
using Darkengines.Authentication.Entities;
using Darkengines.Applications.Entities;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Darkengines.Authentication {
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
		protected SmtpClientFactory SmtpClientFactory { get; }
		protected RazorLightEngine RazorEngine { get; }

		public Authentication(
			JwtAuthenticationConfiguration jwtAuthenticationConfiguration,
			IConfiguration configuration,
			JsonSerializer jsonSerializer,
			ILogger<Authentication> logger,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AuthenticationOptions> options,
			ApplicationDbContext applicationDbContext,
			IIdentityProvider identityProvider,
			SmtpClientFactory smtpClientFactory,
			RazorLightEngine razorEngine
		) {
			ApplicationDbContext = applicationDbContext;
			IdentityProvider = identityProvider;
			Logger = logger;
			HttpContextAccessor = httpContextAccessor;
			AuthenticationOptions = options.Value;
			JwtAuthenticationConfiguration = jwtAuthenticationConfiguration;
			Configuration = configuration;
			RazorEngine = razorEngine;
			JsonSerializer = jsonSerializer;
			SmtpClientFactory = smtpClientFactory;

			JwtJsonSerializer = new JsonSerializer();
			JwtJsonSerializer.ContractResolver = new JwtPropertyContractResolver();
			JwtJsonSerializer.TypeNameHandling = TypeNameHandling.Objects;
			JwtJsonSerializer.Formatting = Formatting.Indented;

		}
		public static TimeSpan UserEmailAddressGuidTimeSpan { get; } = new TimeSpan(1, 0, 0, 0);
		public async Task RequestUserEmailAddressVerificationAsync(byte[] hashedEmailAddress, Uri emailAddressConfirmationUri) {
			var userEmailAddress = new UserEmailAddress {
				HashedEmailAddress = hashedEmailAddress
			};
			var entry = ApplicationDbContext.Attach(userEmailAddress);
			userEmailAddress.Guid = Guid.NewGuid();
			userEmailAddress.GuidExpirationDate = DateTimeOffset.UtcNow.Add(UserEmailAddressGuidTimeSpan);
			entry.DetectChanges();

			await ApplicationDbContext.SaveChangesAsync();
			await entry.ReloadAsync();
			await SendEmailAddressVerificationAsync(userEmailAddress, new Uri(emailAddressConfirmationUri, userEmailAddress.Guid.ToString()));
		}
		public async Task<string> VerifyAsync(Guid guid) {
			var userEmailAddress = await ApplicationDbContext.UserEmailAddresses
				.Include(userEmailAddress => userEmailAddress.User)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.Guid == guid
					&& !userEmailAddress.IsVerified
				);
			if (userEmailAddress == null || userEmailAddress.GuidExpirationDate < DateTimeOffset.UtcNow.Subtract(UserEmailAddressGuidTimeSpan)) throw new NotFoundException();
			userEmailAddress.IsVerified = true;
			userEmailAddress.User.IsVerified = true;
			var token = await BuildToken(userEmailAddress.User);
			await ApplicationDbContext.SaveChangesAsync();
			return token;
		}
		public async Task SendEmailAddressVerificationAsync(UserEmailAddress userEmailAddress, Uri emailAddressConfirmationUri) {
			var template = await RazorEngine.CompileRenderAsync(
				$"{typeof(EmailAddressConfirmation).Namespace}.{nameof(EmailAddressConfirmation)}",
				new EmailAddressConfirmationModel {
					UserEmailAddress = userEmailAddress,
					EmailAddressConfirmationUri = emailAddressConfirmationUri
				}
			);
			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = template;
			bodyBuilder.TextBody = userEmailAddress.EmailAddress;
			var body = bodyBuilder.ToMessageBody();
			var message = new MimeMessage() {
				Sender = new MailboxAddress("Support", "support@darkengines.com"),
				Subject = "Hello world!",
				Body = body
			};
			message.To.Add(new MailboxAddress(userEmailAddress.EmailAddress, userEmailAddress.EmailAddress));

			using var smtpClient = await SmtpClientFactory.CreateSmtpClient();
			await smtpClient.SendAsync(message);
			await smtpClient.DisconnectAsync(true);
		}
		public async Task<string> Create(string login, string email, string password, Uri emailAddressVerificationUri) {
			if (!AuthenticationOptions.AllowedClientUris.Any(allowedClientUri => allowedClientUri.Host == emailAddressVerificationUri.Host)) throw new Exception($"Host {emailAddressVerificationUri.Host} is not allowed.");
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
				Login = login,
				HashedPassword = hashedPassword,
			};
			userEmailAddress = new UserEmailAddress {
				EmailAddress = email,
				HashedEmailAddress = hashedEmailAddress,
				Guid = Guid.NewGuid(),
				GuidExpirationDate = DateTimeOffset.UtcNow.Add(UserEmailAddressGuidTimeSpan),
			};
			user.UserEmailAddresses.Add(userEmailAddress);

			await ApplicationDbContext.Users.AddAsync(user);
			await ApplicationDbContext.SaveChangesAsync();

			await SendEmailAddressVerificationAsync(userEmailAddress, new Uri(emailAddressVerificationUri, userEmailAddress.Guid.ToString()));

			return await BuildToken(user);
		}
		public async Task<string> Login(string email, string password) {
			var userEmailAddress = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.HashedEmailAddress.SequenceEqual(ToLowerInvariantSHA256(email))
				);
			if (userEmailAddress != null) {
				byte[] hashedPassword = ToSHA256(password);
				userEmailAddress.User.IsVerified = userEmailAddress.IsVerified;
				if (userEmailAddress.User.HashedPassword.SequenceEqual(hashedPassword)) return await BuildToken(userEmailAddress.User);
			}
			throw new InvalidCredentialException("Invalid credentials");
		}
		public async Task DeleteAsync() {
			User user = await ApplicationDbContext.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.Id == IdentityProvider.GetIdentity().User.Id);
			user.IsActive = false;
			ApplicationDbContext.SaveChanges();
		}
		public async Task<string> BuildToken(User user) {
			var unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0));
			var now = DateTimeOffset.UtcNow;
			var payload = new JObject() {
				{ "sub", user.Id },
				{ "iss", JwtAuthenticationConfiguration.Parameters.ValidIssuer },
				{ "iat", Math.Floor((now - unixEpoch).TotalSeconds) },
				{ "nbf", Math.Floor((now - unixEpoch).TotalSeconds) },
				{ "exp", Math.Floor((now.AddYears(1) - unixEpoch).TotalSeconds) },
				{ "user", JObject.FromObject(user,  JwtJsonSerializer) }
			};
			//var jwt = new JwtSecurityToken(JwtAuthenticationConfiguration.JwtHeader, payload);
			using var stringWriter = new StringWriter();
			JsonSerializer.Serialize(stringWriter, payload);
			var token = JwtAuthenticationConfiguration.JwtSecurityTokenHandler.CreateToken(stringWriter.ToString(), JwtAuthenticationConfiguration.SigningCredentials);
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

		public async Task SendPasswordResetTokenAsync(UserPasswordResetRequest userPasswordResetRequest, Uri EmailAddressConfirmationUri) {
			var template = await RazorEngine.CompileRenderAsync(
				$"{typeof(UserPasswordResetToken).Namespace}.{nameof(UserPasswordResetToken)}",
				new UserPasswordResetTokenModel { PasswordResetUri = EmailAddressConfirmationUri, Token = userPasswordResetRequest.Guid }
			);
			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = template;
			bodyBuilder.TextBody = string.Empty;
			var body = bodyBuilder.ToMessageBody();
			var message = new MimeMessage() {
				Sender = new MailboxAddress("Support", "support@darkengines.com"),
				Subject = "Hello world!",
				Body = body
			};

			foreach (var userEmailAddress in userPasswordResetRequest.User.UserEmailAddresses) {
				message.To.Add(new MailboxAddress(userEmailAddress.EmailAddress, userEmailAddress.EmailAddress));
			}

			using var smtpClient = await SmtpClientFactory.CreateSmtpClient();
			await smtpClient.SendAsync(message);
			await smtpClient.DisconnectAsync(true);
		}

		public async Task RequestPasswordReset(string email, Uri resetPasswordUri) {
			if (!AuthenticationOptions.AllowedClientUris.Any(allowedClientUri => allowedClientUri.Host == resetPasswordUri.Host)) return;
			var user = await ApplicationDbContext.Users
				.Include(user => user.UserEmailAddresses)
				.Include(user => user.UserPasswordResetRequests)
				.FirstOrDefaultAsync(user =>
					user.Login == email
					|| user.UserEmailAddresses.Any(userEmailAddress => userEmailAddress.IsVerified && userEmailAddress.EmailAddress == email)
				);
			if (user != null) {
				var request = new UserPasswordResetRequest {
					Guid = Guid.NewGuid(),
					User = user
				};
				await ApplicationDbContext.AddAsync(request);
				await ApplicationDbContext.SaveChangesAsync();

				await SendPasswordResetTokenAsync(request, new Uri(resetPasswordUri, request.Guid.ToString()));
			}
		}

		public async Task ResetPassword(Guid guid, string password) {
			var userPasswordResetRequest = await ApplicationDbContext.UserPasswordResetRequests
			.Include(userPasswordResetRequest => userPasswordResetRequest.User)
			.OrderByDescending(userPasswordResetRequest => userPasswordResetRequest.CreatedOn)
			.FirstOrDefaultAsync(userPasswordResetRequest =>
				userPasswordResetRequest.Guid == guid
				&& userPasswordResetRequest.CreatedOn.Value.AddHours(1) > DateTimeOffset.Now
			);

			if (userPasswordResetRequest != null) {
				userPasswordResetRequest.User.HashedPassword = password.ToSHA256();
			}

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