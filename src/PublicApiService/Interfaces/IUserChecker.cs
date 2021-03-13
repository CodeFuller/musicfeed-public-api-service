using System.Threading;
using System.Threading.Tasks;
using idunno.Authentication.Basic;

namespace PublicApiService.Interfaces
{
	public interface IUserChecker
	{
		Task CheckUser(ValidateCredentialsContext context, CancellationToken cancellationToken);
	}
}
