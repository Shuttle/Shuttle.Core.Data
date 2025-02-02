using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DataRepository<T> : IDataRepository<T> where T : class
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IDataRowMapper<T> _dataRowMapper;

    public DataRepository(IDatabaseContextService databaseContextService, IDataRowMapper<T> dataRowMapper)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _dataRowMapper = Guard.AgainstNull(dataRowMapper);
    }

    public async Task<IEnumerable<T>> FetchItemsAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        var rows = await _databaseContextService.Active.GetRowsAsync(query, cancellationToken);

        return await Task.FromResult(rows.MappedRowsUsing(_dataRowMapper).Select(row => row.Result).OfType<T>()).ConfigureAwait(false);
    }

    public async Task<T?> FetchItemAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        var row = await _databaseContextService.Active.GetRowAsync(query, cancellationToken);

        return await Task.FromResult(row == null ? default : _dataRowMapper.Map(row).Result);
    }

    public async Task<MappedRow<T>?> FetchMappedRowAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        var row = await _databaseContextService.Active.GetRowAsync(query, cancellationToken);

        return row == null ? null : _dataRowMapper.Map(row);
    }

    public async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        var rows = await _databaseContextService.Active.GetRowsAsync(query, cancellationToken);

        return rows.MappedRowsUsing(_dataRowMapper);
    }

    public async Task<bool> ContainsAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        return await _databaseContextService.Active.GetScalarAsync<int>(query, cancellationToken) == 1;
    }
}