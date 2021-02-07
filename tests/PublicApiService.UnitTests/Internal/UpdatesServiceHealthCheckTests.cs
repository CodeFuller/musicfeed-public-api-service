using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using PublicApiService.Internal;
using UpdatesService.Client;
using UpdatesService.Grpc;

namespace PublicApiService.UnitTests.Internal
{
	[TestClass]
	public class UpdatesServiceHealthCheckTests
	{
		[TestMethod]
		public async Task CheckHealthAsync_CheckThrowsRpcException_ReturnsUnhealthyCheckResult()
		{
			// Arrange

			var healthServiceClientStub = new Mock<IHealthServiceClient>();
			healthServiceClientStub.Setup(x => x.CheckAsync(It.IsAny<HealthCheckRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
				.Throws(new RpcException(new Status(StatusCode.Internal, "Oops")));

			var mocker = new AutoMocker();
			mocker.Use(healthServiceClientStub);

			var target = mocker.CreateInstance<UpdatesServiceHealthCheck>();

			// Act

			var result = await target.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

			// Assert

			Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
			Assert.AreEqual("Health check for UpdatesService has failed", result.Description);
		}

		[DataRow(HealthCheckResponse.Types.ServingStatus.Unknown)]
		[DataRow(HealthCheckResponse.Types.ServingStatus.NotServing)]
		[DataRow(HealthCheckResponse.Types.ServingStatus.ServiceUnknown)]
		[DataTestMethod]
		public async Task CheckHealthAsync_CheckReturnsFailedStatus_ReturnsUnhealthyCheckResult(HealthCheckResponse.Types.ServingStatus status)
		{
			// Arrange

			using var callResult = new HealthCheckResponse { Status = status }.ToAsyncUnaryCall();

			var healthServiceClientStub = new Mock<IHealthServiceClient>();
			healthServiceClientStub.Setup(x => x.CheckAsync(It.IsAny<HealthCheckRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
				.Returns(callResult);

			var mocker = new AutoMocker();
			mocker.Use(healthServiceClientStub);

			var target = mocker.CreateInstance<UpdatesServiceHealthCheck>();

			// Act

			var result = await target.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

			// Assert

			Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
			Assert.AreEqual($"Health check for UpdatesService returned {status} status", result.Description);
		}
	}
}
