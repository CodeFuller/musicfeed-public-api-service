using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicFeed.PublicApiService.Models;

namespace MusicFeed.PublicApiService.Interfaces
{
	public interface INewReleasesProvider
	{
		Task<IReadOnlyCollection<ReleaseModel>> GetNewReleases(ApiUserModel apiUser, CancellationToken cancellationToken);
	}
}
