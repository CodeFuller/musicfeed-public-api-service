using System.Threading;
using System.Threading.Tasks;
using idunno.Authentication.Basic;

namespace MusicFeed.PublicApiService.Interfaces
{
	public interface IUserChecker
	{
		Task CheckUser(ValidateCredentialsContext context, CancellationToken cancellationToken);
	}
}
