using GraphQL.Types;
using PublicApiService.Models;

namespace PublicApiService.GraphQL.Types
{
	public class SettingsType : ObjectGraphType<SettingsModel>
	{
		public SettingsType()
		{
			Field(x => x.UpdatesServiceAddress, nullable: true);
		}
	}
}
