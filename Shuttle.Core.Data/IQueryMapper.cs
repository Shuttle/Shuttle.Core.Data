using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IQueryMapper
    {
        Task<MappedRow<T>> MapRow<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<MappedRow<T>>> MapRows<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapObject<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<IEnumerable<T>> MapObjects<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default) where T : new();
        Task<T> MapValue<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> MapValues<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<dynamic> MapItem(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<IEnumerable<dynamic>> MapItems(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    }
}