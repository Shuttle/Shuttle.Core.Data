using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        MappedRow<T> MapRow<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        IEnumerable<MappedRow<T>> MapRows<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        T MapObject<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        IEnumerable<T> MapObjects<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        T MapValue<T>(IQuery query, CancellationToken cancellationToken = default);
        IEnumerable<T> MapValues<T>(IQuery query, CancellationToken cancellationToken = default);
        dynamic MapItem(IQuery query, CancellationToken cancellationToken = default);
        IEnumerable<dynamic> MapItems(IQuery query, CancellationToken cancellationToken = default);

        Task<MappedRow<T>> MapRowAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapObjectAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<T>> MapObjectsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapValueAsync<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> MapValuesAsync<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<dynamic> MapItemAsync(IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<dynamic>> MapItemsAsync(IQuery query, CancellationToken cancellationToken = default);
    }
}