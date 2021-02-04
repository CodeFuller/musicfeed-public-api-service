namespace PublicApiService.IntegrationTests.Responses
{
	public record NewReleaseData
	{
		public string Id { get; init; }

		public int? Year { get; init; }

		public string Title { get; init; }
	}
}
