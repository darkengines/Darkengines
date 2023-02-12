using Darkengines.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace Darkengines.Identity {
	public class AuthorizationController : Controller {
		ApplicationDbContext ApplicationDbContext { get; }
		public AuthorizationController(ApplicationDbContext applicationDbContext) {
			ApplicationDbContext = applicationDbContext;
		}
		[HttpGet("~/authorize/")]
		public async Task<AuthorizationResponse> Authorize(ResourceOwnerPasswordCredentialsRequest request) {
			var userEmailAddress = await ApplicationDbContext.UserEmailAddresses.Include(userEmailAddress => userEmailAddress.User)
				.OrderByDescending(userEmailAddress => userEmailAddress.UserId)
				.FirstOrDefaultAsync(userEmailAddress =>
					userEmailAddress.IsVerified
					&& userEmailAddress.HashedEmailAddress == Authentication.Authentication.ToLowerInvariantSHA256(request.Username)
				);
			if (userEmailAddress != null) {
				byte[] hashedPassword = Authentication.Authentication.ToSHA256(request.Password);
				//if (userEmailAddress.User.HashedPassword.SequenceEqual(hashedPassword)) return await BuildToken(userEmailAddress.User);
			}
			return new AuthorizationResponse();
		}
	}
}
