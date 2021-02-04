using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PublicApiService.Interfaces;
using PublicApiService.Models;
using UpdatesService.Client;
using UpdatesService.Grpc;

namespace PublicApiService.Internal
{
	public class NewReleasesProvider : INewReleasesProvider
	{
		private readonly IUpdatesServiceClient serviceClient;

		public NewReleasesProvider(IUpdatesServiceClient serviceClient)
		{
			this.serviceClient = serviceClient ?? throw new ArgumentNullException(nameof(serviceClient));
		}

		public async Task<IReadOnlyCollection<ReleaseModel>> GetNewReleases(CancellationToken cancellationToken)
		{
			var request = new NewReleasesRequest
			{
				UserId = "TestUser",
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
