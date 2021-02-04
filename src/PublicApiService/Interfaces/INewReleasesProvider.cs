using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PublicApiService.Models;

namespace PublicApiService.Interfaces
{
	public interface INewReleasesProvider
	{
		Task<IReadOnlyCollection<ReleaseModel>> GetNewReleases(CancellationToken cancellationToken);
	}
}
