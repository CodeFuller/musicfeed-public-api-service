using System;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace PublicApiService.IntegrationTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private readonly Action<IServiceCollection> setupServices;

		public CustomWebApplicationFactory(Action<IServiceCollection> setupServices = null)
		{
			this.setupServices = setupServices ?? (_ => { });
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureAppConfiguration(ConfigurationProvider.ApplyConfiguration);
			builder.ConfigureServices(setupServices);
		}

		public IGraphQLClient CreateGraphQLClient()
		{
			var httpClient = CreateClient();

			var clientOptions = new GraphQLHttpClientOptions
			{
				EndPoint = new Uri(httpClient.BaseAddress, "graphql"),
			};

			return new GraphQLHttpClient(clientOptions, new NewtonsoftJsonSerializer(), httpClient);
		}
	}
}
