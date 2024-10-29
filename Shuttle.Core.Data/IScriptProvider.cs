using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data;

public interface IScriptProvider
{
    ValueTask<string> GetAsync(string connectionStringName, string scriptName, CancellationToken cancellationToken = default);
}