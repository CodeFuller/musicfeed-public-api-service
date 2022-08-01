using System.Threading;
using System.Threading.Tasks;
using MusicFeed.PublicApiService.Interfaces;

namespace MusicFeed.PublicApiService.Internal
{
	public class UserAuthenticator : IUserAuthenticator
	{
		public async Task<string> AuthenticateUser(string userName, string userPassword, CancellationToken cancellationToken)
		{
			await Task.CompletedTask;

			// TBD: Replace with actual user authentication
			// TBD: Add caching
			if (userName == "TestUser" && userPassword == "TestPassword")
			{
				return "{B9EBE7FC-DAD3-4B2B-B2D9-7E5B058EA670}";
			}

			return null;
		}
	}
}
