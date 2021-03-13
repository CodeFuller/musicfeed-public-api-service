using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiService.IntegrationTests.Responses;

namespace PublicApiService.IntegrationTests.Tests
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

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();
			client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "VGVzdFVzZXI6VGVzdFBhc3N3b3Jk");

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			var data = response.Verify();

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

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			Assert.IsNull(response.Data);

			Assert.IsNotNull(response.Errors);
			Assert.AreEqual(1, response.Errors.Length);

			var error = response.Errors.Single();

			object errorCodeValue = null;
			error.Extensions?.TryGetValue("code", out errorCodeValue);
			Assert.AreEqual("authorization", errorCodeValue);

			Assert.AreEqual("GraphQL.Validation.ValidationError: You are not authorized to run this operation.\r\nThe current user must be authenticated.", error.Message);
		}

		[TestMethod]
		public async Task NewReleasesQuery_ForUnknownUser_ReturnsError()
		{
			// Arrange

			var request = new GraphQLRequest
			{
				Query = @"{ newReleases { id } }",
			};

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();
			client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "WGVzdFVzZXI6VGVzdFBhc3N3b3Jk");

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			Assert.IsNull(response.Data);

			Assert.IsNotNull(response.Errors);
			Assert.AreEqual(1, response.Errors.Length);

			var error = response.Errors.Single();

			object errorCodeValue = null;
			error.Extensions?.TryGetValue("code", out errorCodeValue);
			Assert.AreEqual("authorization", errorCodeValue);

			Assert.AreEqual("GraphQL.Validation.ValidationError: You are not authorized to run this operation.\r\nThe current user must be authenticated.", error.Message);
		}
	}
}
