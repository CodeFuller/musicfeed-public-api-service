using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MusicFeed.PublicApiService.IntegrationTests
{
	public static class ConfigurationProvider
	{
		private static bool RunsInsideContainer => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

		public static void ApplyConfiguration(IConfigurationBuilder configBuilder)
		{
			if (RunsInsideContainer)
			{
				configBuilder
					.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("services:updatesServiceAddress", "http://updates-service:81/") });
			}
		}
	}
}
