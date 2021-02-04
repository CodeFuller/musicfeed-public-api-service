using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace PublicApiService.GraphQL
{
	public class ApiSchema : Schema
	{
		public ApiSchema(IServiceProvider services)
			: base(services)
		{
			Query = services.GetRequiredService<ApiQuery>();
		}
	}
}
