using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        Task<MappedRow<T>> MapRow<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new();
        Task<IEnumerable<MappedRow<T>>> MapRows<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new();
        Task<T> MapObject<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new();
        Task<IEnumerable<T>> MapObjects<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new();
        Task<T> MapValue<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken());
        Task<IEnumerable<T>> MapValues<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken());
        Task<dynamic> MapItem(IQuery query, CancellationToken cancellationToken = new CancellationToken());
        Task<IEnumerable<dynamic>> MapItems(IQuery query, CancellationToken cancellationToken = new CancellationToken());
    }
}