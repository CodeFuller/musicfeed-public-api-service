using System;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace PublicApiService.GraphQL
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
