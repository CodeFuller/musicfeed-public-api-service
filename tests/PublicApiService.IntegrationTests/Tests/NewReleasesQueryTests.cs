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
		public async Task NewReleasesQuery_ReturnsCorrectData()
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

			// Act

			var response = await client.SendQueryAsync<NewReleasesResponse>(request);

			// Assert

			var data = response.Verify();

			data.NewReleases.Should().Equal(expectedData);
		}
	}
}
