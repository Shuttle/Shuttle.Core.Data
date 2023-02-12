using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<IEnumerable<T>> FetchItems(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<T> FetchItem(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<MappedRow<T>> FetchMappedRow(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<MappedRow<T>>> FetchMappedRows(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<bool> Contains(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    }
}