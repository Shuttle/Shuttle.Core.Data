using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data;

public interface IAssembler<T> where T : class
{
    Task<IEnumerable<T>> AssembleAsync(MappedData data, CancellationToken cancellationToken = default);
}