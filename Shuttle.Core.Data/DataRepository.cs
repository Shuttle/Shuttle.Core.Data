using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DataRepository<T> : IDataRepository<T> where T : class
	{
		private readonly IDatabaseGateway _databaseGateway;
		private readonly IDataRowMapper<T> _dataRowMapper;

		public DataRepository(IDatabaseGateway databaseGateway, IDataRowMapper<T> dataRowMapper)
		{
			Guard.AgainstNull(databaseGateway, "databaseGateway");
			Guard.AgainstNull(dataRowMapper, "dataRowMapper");

			_databaseGateway = databaseGateway;
			_dataRowMapper = dataRowMapper;
		}

		public IEnumerable<T> FetchAllUsing(IQuery query)
		{
			return _databaseGateway.GetRowsUsing(query).MappedRowsUsing(_dataRowMapper).Select(row => row.Result).ToList();
		}

		public T FetchItemUsing(IQuery query)
		{
			var row = _databaseGateway.GetSingleRowUsing(query);

			return row == null ? default(T) : _dataRowMapper.Map(row).Result;
		}

		public MappedRow<T> FetchMappedRowUsing(IQuery query)
		{
			var row = _databaseGateway.GetSingleRowUsing(query);

			return row == null ? null : _dataRowMapper.Map(row);
		}

		public IEnumerable<MappedRow<T>> FetchMappedRowsUsing(IQuery query)
		{
			return _databaseGateway.GetRowsUsing(query).MappedRowsUsing(_dataRowMapper);
		}

		public bool Contains(IQuery query)
		{
			return (_databaseGateway.GetScalarUsing<int>(query) == 1);
		}
	}
}