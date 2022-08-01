using GraphQL.Types;
using MusicFeed.PublicApiService.Models;

namespace MusicFeed.PublicApiService.GraphQL.Types
{
	public class ReleaseType : ObjectGraphType<ReleaseModel>
	{
		public ReleaseType()
		{
			Field("id", x => x.Id.Value);
			Field(x => x.Year, nullable: true);
			Field(x => x.Title);
		}
	}
}
