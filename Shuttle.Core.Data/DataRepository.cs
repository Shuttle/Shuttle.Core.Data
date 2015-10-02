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

		public IEnumerable<T> FetchAllUsing(DataSource source, IQuery query)
		{
			return _databaseGateway.GetRowsUsing(source, query).MappedRowsUsing(_dataRowMapper).Select(row => row.Result).ToList();
		}

		public T FetchItemUsing(DataSource source, IQuery query)
		{
			var row = _databaseGateway.GetSingleRowUsing(source, query);

			return row == null ? default(T) : _dataRowMapper.Map(row).Result;
		}

		public MappedRow<T> FetchMappedRowUsing(DataSource source, IQuery query)
		{
			var row = _databaseGateway.GetSingleRowUsing(source, query);

			return row == null ? null : _dataRowMapper.Map(row);
		}

		public IEnumerable<MappedRow<T>> FetchMappedRowsUsing(DataSource source, IQuery query)
		{
			return _databaseGateway.GetRowsUsing(source, query).MappedRowsUsing(_dataRowMapper);
		}

		public bool Contains(DataSource source, IQuery query)
		{
			return (_databaseGateway.GetScalarUsing<int>(source, query) == 1);
		}
	}
}