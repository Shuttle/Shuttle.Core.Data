using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
	    IEnumerable<T> FetchItems(IQuery query);
	    T FetchItem(IQuery query);
	    MappedRow<T> FetchMappedRow(IQuery query);
	    IEnumerable<MappedRow<T>> FetchMappedRows(IQuery query);
	    bool Contains(IQuery query);

	    Task<IEnumerable<T>> FetchItemsAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<T> FetchItemAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<MappedRow<T>> FetchMappedRowAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<bool> ContainsAsync(IQuery query, CancellationToken cancellationToken = default);
    }
}