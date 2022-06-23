using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DataRepository<T> : IDataRepository<T> where T : class
	{
		private readonly IDatabaseGateway _databaseGateway;
		private readonly IDataRowMapper<T> _dataRowMapper;

		public DataRepository(IDatabaseGateway databaseGateway, IDataRowMapper<T> dataRowMapper)
		{
			Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
			Guard.AgainstNull(dataRowMapper, nameof(dataRowMapper));

			_databaseGateway = databaseGateway;
			_dataRowMapper = dataRowMapper;
		}

		public IEnumerable<T> FetchItems(IQuery query)
		{
			return _databaseGateway.GetRows(query).MappedRowsUsing(_dataRowMapper).Select(row => row.Result).ToList();
		}

		public T FetchItem(IQuery query)
		{
			var row = _databaseGateway.GetRow(query);

			return row == null ? default(T) : _dataRowMapper.Map(row).Result;
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

		public bool Contains(IQuery query)
		{
			return (_databaseGateway.GetScalar<int>(query) == 1);
		}
	}
}