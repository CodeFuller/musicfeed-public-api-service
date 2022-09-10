namespace MusicFeed.PublicApiService.Settings
{
	public class AppSettings
	{
		public ServicesSettings Services { get; set; } = new();

		public JwtSettings JwtSettings { get; set; } = new();
	}
}
