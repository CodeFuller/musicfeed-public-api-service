using System;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace MusicFeed.PublicApiService.GraphQL
{
	public class ApiSchema : Schema
	{
		public ApiSchema(IServiceProvider services)
			: base(services)
		{
			this.AuthorizeWith("IsAuthenticated");

			Query = services.GetRequiredService<ApiQuery>();
		}
	}
}
