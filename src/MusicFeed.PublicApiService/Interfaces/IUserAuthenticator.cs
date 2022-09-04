using System.Threading;
using System.Threading.Tasks;

namespace MusicFeed.PublicApiService.Interfaces
{
	public interface IUserAuthenticator
	{
		Task<string> AuthenticateUser(string userName, string userPassword, CancellationToken cancellationToken);
	}
}
