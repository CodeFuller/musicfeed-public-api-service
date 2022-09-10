using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicFeed.PublicApiService.IntegrationTests.Responses;
using MusicFeed.PublicApiService.Settings;

namespace MusicFeed.PublicApiService.IntegrationTests.Tests
{
	[TestClass]
	public class NewReleasesQueryTests
	{
		[TestMethod]
		public async Task NewReleasesQuery_ForExistingUser_ReturnsCorrectData()
		{
			// Arrange

			var expectedData = new[]
			{
				new NewReleaseData
				{
					Id = "1",
					Year = 2000,
					Title = "Don't Give Me Names",
				},

				new NewReleaseData
				{
					Id = "2",
					Year = 2009,
					Title = "Shallow Life",
				},

				new NewReleaseData
				{
					Id = "3",
					Year = 1998,
					Title = "How To Measure A Planet",
				},
			};

			var request = new GraphQLRequest
			{
				Query = @"
				{
					newReleases {
						id
						year
						title
					}
				}",
			};

			await using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();
			await AddAuthorizationToClient(client, factory.Services);

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			var data = response.VerifySuccessfulResponse();

			data.NewReleases.Should().Equal(expectedData);
		}

		[TestMethod]
		public async Task NewReleasesQuery_ForUnauthenticatedRequest_ReturnsError()
		{
			// Arrange

			var request = new GraphQLRequest
			{
				Query = @"{ newReleases { id } }",
			};

			await using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			var expectedError = new StringBuilder();
			expectedError.AppendLine("GraphQL.Validation.ValidationError: You are not authorized to run this operation.");
			expectedError.AppendLine("The current user must be authenticated.");
			expectedError.Append("Required claim 'scope' with any value of 'musicfeed-api' is not present.");

			response.VerifyFailedResponse("authorization", expectedError.ToString());
		}

		private static async Task AddAuthorizationToClient(GraphQLHttpClient client, IServiceProvider serviceProvider)
		{
			var jwt = await IssueJwt(serviceProvider);

			client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
		}

		private static async Task<string> IssueJwt(IServiceProvider serviceProvider)
		{
			using var httpClient = new HttpClient();

			var issueTokenRequest = new
			{
				UserId = "Some User Id",
			};

			var identityServiceAddress = GetIdentityServiceAddress(serviceProvider);

			var stubTokenAddress = new Uri(identityServiceAddress, "stub/token");
			using var issueTokenResponse = await httpClient.PostAsJsonAsync(stubTokenAddress, issueTokenRequest);
			issueTokenResponse.EnsureSuccessStatusCode();

			return await issueTokenResponse.Content.ReadAsStringAsync();
		}

		private static Uri GetIdentityServiceAddress(IServiceProvider serviceProvider)
		{
			var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

			return settings.Services.IdentityServiceAddress;
		}
	}
}
