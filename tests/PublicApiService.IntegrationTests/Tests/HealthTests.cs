using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UpdatesService.Client;
using UpdatesService.Grpc;

namespace PublicApiService.IntegrationTests.Tests
{
	[TestClass]
	public class HealthTests
	{
		[TestMethod]
		public async Task LiveRequest_AllServicesAreUp_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task LiveRequest_UpdatesServiceIsDown_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory(StubFailingUpdatesService());
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task ReadyRequest_AllServicesAreUp_ReturnsHealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory();
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public async Task ReadyRequest_UpdatesServiceIsDown_ReturnsUnhealthyResponse()
		{
			// Arrange

			using var factory = new CustomWebApplicationFactory(StubFailingUpdatesService());
			using var client = factory.CreateClient();

			// Act

			var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

			// Assert

			Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
		}

		private static Action<IServiceCollection> StubFailingUpdatesService()
		{
			var healthServiceClientStub = new Mock<IHealthServiceClient>();
			healthServiceClientStub.Setup(x => x.CheckAsync(It.IsAny<HealthCheckRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
				.Throws(new RpcException(new Status(StatusCode.Internal, "Oops")));

			return services => services.AddSingleton(healthServiceClientStub.Object);
		}
	}
}
