using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UpdatesService.Client;
using UpdatesService.Grpc;

namespace PublicApiService.Internal
{
	public class UpdatesServiceHealthCheck : IHealthCheck
	{
		private readonly IHealthServiceClient healthServiceClient;

		private readonly ILogger<UpdatesServiceHealthCheck> logger;

		public UpdatesServiceHealthCheck(IHealthServiceClient healthServiceClient, ILogger<UpdatesServiceHealthCheck> logger)
		{
			this.healthServiceClient = healthServiceClient ?? throw new ArgumentNullException(nameof(healthServiceClient));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
		{
			var request = new HealthCheckRequest
			{
				Service = String.Empty,
			};

			try
			{
				var response = await healthServiceClient.CheckAsync(request, deadline: DateTime.UtcNow.AddSeconds(1), cancellationToken: cancellationToken);

				if (response.Status == HealthCheckResponse.Types.ServingStatus.Serving)
				{
					return HealthCheckResult.Healthy();
				}

				logger.LogWarning("Health check for {ServiceName} returned {HealthCheckStatus}", "UpdatesService", response.Status);
				return HealthCheckResult.Unhealthy($"Health check for UpdatesService returned {response.Status} status");
			}
			catch (RpcException e)
			{
				logger.LogWarning(e, "Health check for {ServiceName} has failed", "UpdatesService");
				return HealthCheckResult.Unhealthy("Health check for UpdatesService has failed", e);
			}
		}
	}
}
