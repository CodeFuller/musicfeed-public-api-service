using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicFeed.PublicApiService.Interfaces;
using MusicFeed.PublicApiService.Models;
using MusicFeed.UpdatesService.Client;
using MusicFeed.UpdatesService.Grpc;

namespace MusicFeed.PublicApiService.Internal
{
	public class NewReleasesProvider : INewReleasesProvider
	{
		private readonly IUpdatesServiceClient serviceClient;

		public NewReleasesProvider(IUpdatesServiceClient serviceClient)
		{
			this.serviceClient = serviceClient ?? throw new ArgumentNullException(nameof(serviceClient));
		}

		public async Task<IReadOnlyCollection<ReleaseModel>> GetNewReleases(ApiUserModel apiUser, CancellationToken cancellationToken)
		{
			var request = new NewReleasesRequest
			{
				UserId = apiUser.Id,
			};

			var response = await serviceClient.GetNewReleasesAsync(request, cancellationToken: cancellationToken);

			return response.NewReleases.Select(x => new ReleaseModel
				{
					Id = new IdModel(x.Id),
					Year = x.Year,
					Title = x.Title,
				})
				.ToList();
		}
	}
}
