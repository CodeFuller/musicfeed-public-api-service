using System;
using System.Linq;
using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PublicApiService.IntegrationTests
{
	public static class GraphQLResponseExtensions
	{
		public static TResponse Verify<TResponse>(this GraphQLResponse<TResponse> response)
		{
			if (response.Errors?.Any() ?? false)
			{
				var errors = response.Errors.Select((e, i) => $"Error {i + 1}: {e.Message}");
				Assert.Fail($"GraphQL response contains error(s):\n\n{String.Join("\n", errors)}");
			}

			if (response.Data == null)
			{
				Assert.Fail("GraphQL response contains null data");
			}

			return response.Data;
		}
	}
}
