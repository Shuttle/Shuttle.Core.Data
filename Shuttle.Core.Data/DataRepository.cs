using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IDataRowMapper<T> _dataRowMapper;

        public DataRepository(IDatabaseGateway databaseGateway, IDataRowMapper<T> dataRowMapper)
        {
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _dataRowMapper = Guard.AgainstNull(dataRowMapper, nameof(dataRowMapper));
        }

        public IEnumerable<T> FetchItems(IQuery query)
        {
            return _databaseGateway.GetRows(query).MappedRowsUsing(_dataRowMapper).Select(row => row.Result).ToList();
        }

        public async Task<IEnumerable<T>> FetchItemsAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var rows = await _databaseGateway.GetRowsAsync(query, cancellationToken);

            return (IEnumerable<T>)await Task.FromResult(rows.MappedRowsUsing(_dataRowMapper).Select(row => row.Result)).ConfigureAwait(false);
        }

        public T FetchItem(IQuery query)
        {
            var row = _databaseGateway.GetRow(query);

            return row == null ? default : _dataRowMapper.Map(row).Result;
        }

        public async Task<T> FetchItemAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var row = await _databaseGateway.GetRowAsync(query, cancellationToken);

            return await Task.FromResult(row == null ? default : _dataRowMapper.Map(row).Result);
        }

        public MappedRow<T> FetchMappedRow(IQuery query)
        {
            var row = _databaseGateway.GetRow(query);

            return row == null ? null : _dataRowMapper.Map(row);
        }

        public IEnumerable<MappedRow<T>> FetchMappedRows(IQuery query)
        {
            return _databaseGateway.GetRows(query).MappedRowsUsing(_dataRowMapper);
        }

        public async Task<MappedRow<T>> FetchMappedRowAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var row = await _databaseGateway.GetRowAsync(query, cancellationToken);

            return row == null ? null : _dataRowMapper.Map(row);
        }

        public async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var rows = await _databaseGateway.GetRowsAsync(query, cancellationToken);

            return rows.MappedRowsUsing(_dataRowMapper);
        }

        public bool Contains(IQuery query)
        {
            return _databaseGateway.GetScalar<int>(query) == 1;
        }

        public async Task<bool> ContainsAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            return await _databaseGateway.GetScalarAsync<int>(query, cancellationToken) == 1;
        }
    }
}