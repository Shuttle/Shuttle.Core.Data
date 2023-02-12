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

        public async Task<IEnumerable<T>> FetchItems(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var rows = await _databaseGateway.GetRows(databaseContext, query, cancellationToken);

            return (IEnumerable<T>)await Task.FromResult(rows.MappedRowsUsing(_dataRowMapper).Select(row => row.Result)).ConfigureAwait(false);
        }

        public async Task<T> FetchItem(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var row = await _databaseGateway.GetRow(databaseContext, query, cancellationToken);

            return await Task.FromResult(row == null ? default : _dataRowMapper.Map(row).Result);
        }

        public async Task<MappedRow<T>> FetchMappedRow(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var row = await _databaseGateway.GetRow(databaseContext, query, cancellationToken);

            return row == null ? null : _dataRowMapper.Map(row);
        }

        public async Task<IEnumerable<MappedRow<T>>> FetchMappedRows(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var rows = await _databaseGateway.GetRows(databaseContext, query, cancellationToken);

            return rows.MappedRowsUsing(_dataRowMapper);
        }

        public async Task<bool> Contains(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            return await _databaseGateway.GetScalar<int>(databaseContext, query, cancellationToken) == 1;
        }
    }
}