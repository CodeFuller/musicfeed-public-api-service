using System.Threading;
using System.Threading.Tasks;

namespace PublicApiService.Interfaces
{
	public interface IUserAuthenticator
	{
		Task<string> AuthenticateUser(string userName, string userPassword, CancellationToken cancellationToken);
	}
}
