using System.Threading.Tasks;
using Grpc.Core;

namespace PublicApiService.UnitTests
{
	public static class ObjectExtensions
	{
		public static AsyncUnaryCall<TResponse> ToAsyncUnaryCall<TResponse>(this TResponse response)
		{
			return new(Task.FromResult(response), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
		}
	}
}
