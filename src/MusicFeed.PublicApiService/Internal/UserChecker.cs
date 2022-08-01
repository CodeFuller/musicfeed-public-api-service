using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using idunno.Authentication.Basic;
using Microsoft.Extensions.Logging;
using MusicFeed.PublicApiService.Interfaces;

namespace MusicFeed.PublicApiService.Internal
{
	public class UserChecker : IUserChecker
	{
		private readonly IUserAuthenticator userAuthenticator;

		private readonly ILogger<UserChecker> logger;

		public UserChecker(IUserAuthenticator userAuthenticator, ILogger<UserChecker> logger)
		{
			this.userAuthenticator = userAuthenticator ?? throw new ArgumentNullException(nameof(userAuthenticator));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckUser(ValidateCredentialsContext context, CancellationToken cancellationToken)
		{
			var userName = context.Username;

			logger.LogInformation("Authenticating the user {UserName} ...", userName);

			var userId = await userAuthenticator.AuthenticateUser(userName, context.Password, cancellationToken);
			if (userId != null)
			{
				logger.LogInformation("The user {UserName} was authenticated successfully", userName);

				var claims = new[]
				{
					new Claim(ClaimTypes.NameIdentifier, userId),
					new Claim(ClaimTypes.Name, userName),
				};

				context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
				context.Success();
				return;
			}

			logger.LogWarning("Failed to authenticate user {UserName}", userName);
			context.Fail("The user name or password is incorrect");
		}
	}
}
