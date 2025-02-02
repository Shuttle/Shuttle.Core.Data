using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data;

public interface IQueryMapper
{
    Task<dynamic?> MapItemAsync(IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<dynamic>> MapItemsAsync(IQuery query, CancellationToken cancellationToken = default);
    Task<T?> MapObjectAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<IEnumerable<T>> MapObjectsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<MappedRow<T>?> MapRowAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
    Task<T?> MapValueAsync<T>(IQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> MapValuesAsync<T>(IQuery query, CancellationToken cancellationToken = default);
}