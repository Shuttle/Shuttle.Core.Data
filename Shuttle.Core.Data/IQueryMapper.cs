using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data;

public interface IQueryMapper
{
    Task<dynamic?> MapItemAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<dynamic>> MapItemsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<T?> MapObjectAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<IEnumerable<T>> MapObjectsAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<MappedRow<T>?> MapRowAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<T?> MapValueAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> MapValuesAsync<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
}