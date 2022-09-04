using System;
using System.Linq;
using GraphQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicFeed.PublicApiService.IntegrationTests
{
	public static class GraphQLResponseExtensions
	{
		public static TResponse VerifySuccessfulResponse<TResponse>(this GraphQLResponse<TResponse> response)
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

		public static void VerifyFailedResponse<TResponse>(this GraphQLResponse<TResponse> response, string expectedErrorCode, string expectedErrorMessage)
		{
			Assert.IsNull(response.Data);

			Assert.IsNotNull(response.Errors);
			Assert.AreEqual(1, response.Errors.Length);

			var error = response.Errors.Single();

			object errorCodeValue = null;
			error.Extensions?.TryGetValue("code", out errorCodeValue);
			Assert.AreEqual(expectedErrorCode, errorCodeValue);

			Assert.AreEqual(expectedErrorMessage, error.Message.FixLineEndings());
		}

		private static string FixLineEndings(this string text)
		{
			return text.Replace("\r\n", "\n", StringComparison.Ordinal);
		}
	}
}
