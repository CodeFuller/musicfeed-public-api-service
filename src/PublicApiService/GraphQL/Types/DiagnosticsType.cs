using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using PublicApiService.Models;
using UpdatesService.Client;

namespace PublicApiService.GraphQL.Types
{
	public class DiagnosticsType : ObjectGraphType<DiagnosticsModel>
	{
		public DiagnosticsType(IServiceProvider serviceProvider)
		{
			Field("version", x => x.Version, nullable: true);
			Field<SettingsType>("settings", resolve: context => context.Source.Settings);

			async Task<UpdatesServiceDiagnosticsModel> GetDiagnostics(CancellationToken cancellationToken)
			{
				var serviceClient = serviceProvider.GetRequiredService<IUpdatesDiagnosticsServiceClient>();
				var diagnostics = await serviceClient.GetDiagnosticsAsync(cancellationToken: cancellationToken);
				return new UpdatesServiceDiagnosticsModel
				{
					Version = diagnostics.Version,
				};
			}

			Field<NonNullGraphType<UpdatesServiceDiagnosticsType>>("updatesService", resolve: context => GetDiagnostics(context.CancellationToken));
		}
	}
}
