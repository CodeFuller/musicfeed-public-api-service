using System.Threading;
using System.Threading.Tasks;
using PublicApiService.Models;

namespace PublicApiService.Interfaces
{
	public interface IDiagnosticsProvider
	{
		Task<DiagnosticsModel> GetDiagnostics(CancellationToken cancellationToken);
	}
}
