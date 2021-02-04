using System.Collections.Generic;

namespace PublicApiService.IntegrationTests.Responses
{
	public class NewReleasesResponse
	{
		public IReadOnlyCollection<NewReleaseData> NewReleases { get; init; }
	}
}
