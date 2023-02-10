using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        Task<MappedRow<T>> MapRow<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<MappedRow<T>>> MapRows<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapObject<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<T>> MapObjects<T>(IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapValue<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> MapValues<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<dynamic> MapItem(IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<dynamic>> MapItems(IQuery query, CancellationToken cancellationToken = default);
    }
}