using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using PublicApiService.GraphQL.Types;
using PublicApiService.Interfaces;

namespace PublicApiService.GraphQL
{
	public class ApiQuery : ObjectGraphType
	{
		public ApiQuery(IServiceProvider serviceProvider)
		{
			TProvider GetProvider<TProvider>() => serviceProvider.GetRequiredService<TProvider>();

			FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<ReleaseType>>>>(
				"newReleases",
				resolve: async context => await GetProvider<INewReleasesProvider>().GetNewReleases(context.CancellationToken));
		}
	}
}
