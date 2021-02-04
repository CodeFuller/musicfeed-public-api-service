using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PublicApiService.Interfaces;
using PublicApiService.Models;
using PublicApiService.Settings;

namespace PublicApiService.Internal
{
	public class DiagnosticsProvider : IDiagnosticsProvider
	{
		private readonly AppSettings settings;

		public DiagnosticsProvider(IOptions<AppSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<DiagnosticsModel> GetDiagnostics(CancellationToken cancellationToken)
		{
			var diagnostics = new DiagnosticsModel
			{
				Version = GetApplicationVersion(),
				Settings = new SettingsModel
				{
					UpdatesServiceAddress = settings.Services.UpdatesServiceAddress.OriginalString,
				},
			};

			return Task.FromResult(diagnostics);
		}

		private static string GetApplicationVersion()
		{
			var assembly = typeof(Program).Assembly.Location;
			var versionInfo = FileVersionInfo.GetVersionInfo(assembly);
			return versionInfo.ProductVersion;
		}
	}
}
