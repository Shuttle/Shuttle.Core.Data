using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data;

public interface IDataRepository<T> where T : class
{
    Task<bool> ContainsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<T?> FetchItemAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FetchItemsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<MappedRow<T>?> FetchMappedRowAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
}