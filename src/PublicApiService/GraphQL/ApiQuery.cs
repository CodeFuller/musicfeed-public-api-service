using System;
using System.Linq;
using System.Security.Claims;
using GraphQL.Execution;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using PublicApiService.GraphQL.Types;
using PublicApiService.Interfaces;
using PublicApiService.Models;

namespace PublicApiService.GraphQL
{
	public class ApiQuery : ObjectGraphType
	{
		public ApiQuery(IServiceProvider serviceProvider)
		{
			TProvider GetProvider<TProvider>() => serviceProvider.GetRequiredService<TProvider>();

			FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<ReleaseType>>>>(
				"newReleases",
				resolve: async context =>
				{
					var apiUser = GetApiUserForRequest(context);

					return await GetProvider<INewReleasesProvider>().GetNewReleases(apiUser, context.CancellationToken);
				});
		}

		private static ApiUserModel GetApiUserForRequest(IProvideUserContext context)
		{
			var principal = GetClaimsPrincipalFromContext(context);

			var userId = principal.Claims
				.Where(c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value).SingleOrDefault();

			return new ApiUserModel
			{
				Id = userId,
			};
		}

		private static ClaimsPrincipal GetClaimsPrincipalFromContext(IProvideUserContext context)
		{
			var userContext = context.UserContext;

			if (!userContext.TryGetValue("user", out var userObject))
			{
				throw new InvalidOperationException("User value is missing in UserContext");
			}

			if (userObject == null)
			{
				throw new InvalidOperationException("User value is null in UserContext");
			}

			if (userObject is not ClaimsPrincipal principal)
			{
				throw new InvalidOperationException($"User object has incorrect type {userObject.GetType()}");
			}

			return principal;
		}
	}
}
