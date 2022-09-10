using System;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MusicFeed.PublicApiService.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly Action<IServiceCollection> setupServices;

		private readonly Action<IConfigurationBuilder> setupConfiguration;

		public CustomWebApplicationFactory(Action<IServiceCollection> setupServices = null, Action<IConfigurationBuilder> setupConfiguration = null)
		{
			this.setupServices = setupServices ?? (_ => { });
			this.setupConfiguration = setupConfiguration ?? (_ => { });
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(setupServices);
			builder.ConfigureAppConfiguration(setupConfiguration);
		}

		public GraphQLHttpClient CreateGraphQLClient()
		{
			var httpClient = CreateClient();

			var clientOptions = new GraphQLHttpClientOptions
			{
				EndPoint = new Uri(httpClient.BaseAddress, "api/graphql"),
			};

			return new GraphQLHttpClient(clientOptions, new NewtonsoftJsonSerializer(), httpClient);
		}
	}
}
