using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiService.IntegrationTests.Responses;

namespace PublicApiService.IntegrationTests.Tests
{
	[TestClass]
	public class DiagnosticsQueryTests
	{
		[TestMethod]
		public async Task DiagnosticsQuery_ReturnsCorrectData()
		{
			// Arrange

			var expectedData = new DiagnosticsData
			{
				Settings = new SettingsData
				{
					UpdatesServiceAddress = ConfigurationProvider.UpdatesServiceAddress,
				},

				UpdatesService = new UpdatesServiceDiagnosticsData(),
			};

			var request = new GraphQLRequest
			{
				Query = @"
				{
					diagnostics {
						version
						settings {
							updatesServiceAddress
						}
						updatesService {
							version
						}
					}
				}",
			};

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateGraphQLClient();

			// Act

			var response = await client.SendQueryAsync<DiagnosticsResponse>(request);

			// Assert

			var responseData = response.Verify();

			var diagnostics = responseData.Diagnostics;

			// We do not compare version, because
			// 1. It changes often, which will require frequent update of test data.
			// 2. CI updates version with build number, so we can not predict it in test code.
			diagnostics.Should().BeEquivalentTo(expectedData, o => o.Excluding(x => x.Version).Excluding(x => x.UpdatesService.Version));

			diagnostics.Version.Should().NotBeNullOrEmpty();
			diagnostics.UpdatesService.Version.Should().NotBeNullOrEmpty();
		}
	}
}
