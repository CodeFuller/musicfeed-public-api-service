using GraphQL.Types;
using PublicApiService.Models;

namespace PublicApiService.GraphQL.Types
{
	public class UpdatesServiceDiagnosticsType : ObjectGraphType<UpdatesServiceDiagnosticsModel>
	{
		public UpdatesServiceDiagnosticsType()
		{
			Field("version", x => x.Version, nullable: true);
		}
	}
}
