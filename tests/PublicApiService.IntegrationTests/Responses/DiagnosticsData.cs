namespace PublicApiService.IntegrationTests.Responses
{
	public class DiagnosticsData
	{
		public string Version { get; init; }

		public SettingsData Settings { get; init; }

		public UpdatesServiceDiagnosticsData UpdatesService { get; init; }
	}
}
