using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DataRepository<T> : IDataRepository<T> where T : class
{
    private readonly IDataRowMapper<T> _dataRowMapper;

    public DataRepository(IDataRowMapper<T> dataRowMapper)
    {
        _dataRowMapper = Guard.AgainstNull(dataRowMapper);
    }

    public async Task<IEnumerable<T>> FetchItemsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        var rows = await Guard.AgainstNull(databaseContext).GetRowsAsync(query, cancellationToken);

        return await Task.FromResult(rows.MappedRowsUsing(_dataRowMapper).Select(row => row.Result).OfType<T>()).ConfigureAwait(false);
    }

    public async Task<T?> FetchItemAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        var row = await Guard.AgainstNull(databaseContext).GetRowAsync(query, cancellationToken);

        return await Task.FromResult(row == null ? default : _dataRowMapper.Map(row).Result);
    }

    public async Task<MappedRow<T>?> FetchMappedRowAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        var row = await Guard.AgainstNull(databaseContext).GetRowAsync(query, cancellationToken);

        return row == null ? null : _dataRowMapper.Map(row);
    }

    public async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        var rows = await Guard.AgainstNull(databaseContext).GetRowsAsync(query, cancellationToken);

        return rows.MappedRowsUsing(_dataRowMapper);
    }

    public async Task<bool> ContainsAsync(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
    {
        return await Guard.AgainstNull(databaseContext).GetScalarAsync<int>(query, cancellationToken) == 1;
    }
}