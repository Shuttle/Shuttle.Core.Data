using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<IEnumerable<T>> FetchItems(IQuery query, CancellationToken cancellationToken = default);
		Task<T> FetchItem(IQuery query, CancellationToken cancellationToken = default);
		Task<MappedRow<T>> FetchMappedRow(IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<MappedRow<T>>> FetchMappedRows(IQuery query, CancellationToken cancellationToken = default);
		Task<bool> Contains(IQuery query, CancellationToken cancellationToken = default);
    }
}