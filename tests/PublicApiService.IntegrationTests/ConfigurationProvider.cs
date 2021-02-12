using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PublicApiService.IntegrationTests
{
	public static class ConfigurationProvider
	{
		private static bool RunsInsideContainer => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

		public static string UpdatesServiceAddress => RunsInsideContainer ? "http://updates-service:81/" : "http://localhost:8103/";

		public static void ApplyConfiguration(IConfigurationBuilder configBuilder)
		{
			if (RunsInsideContainer)
			{
				configBuilder
					.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("services:updatesServiceAddress", UpdatesServiceAddress) });
			}
		}
	}
}
