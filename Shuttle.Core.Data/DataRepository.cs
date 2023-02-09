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

        public async Task<IEnumerable<T>> FetchItems(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var rows = await _databaseGateway.GetRows(query, cancellationToken);

            return await Task.FromResult(rows.MappedRowsUsing(_dataRowMapper).Select(row => row.Result).ToList());
        }

        public async Task<T> FetchItem(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var row = await _databaseGateway.GetRow(query, cancellationToken);

            return row == null ? default : _dataRowMapper.Map(row).Result;
        }

        public async Task<MappedRow<T>> FetchMappedRow(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var row = await _databaseGateway.GetRow(query, cancellationToken);

            return row == null ? null : _dataRowMapper.Map(row);
        }

        public async Task<IEnumerable<MappedRow<T>>> FetchMappedRows(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var rows = await _databaseGateway.GetRows(query, cancellationToken);

            return rows.MappedRowsUsing(_dataRowMapper);
        }

        public async Task<bool> Contains(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _databaseGateway.GetScalar<int>(query, cancellationToken) == 1;
        }
    }
}