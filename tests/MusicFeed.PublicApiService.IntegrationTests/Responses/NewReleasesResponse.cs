using System.Collections.Generic;

namespace MusicFeed.PublicApiService.IntegrationTests.Responses
{
	public class NewReleasesResponse
	{
		public IReadOnlyCollection<NewReleaseData> NewReleases { get; init; }
	}
}
