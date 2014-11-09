using System.Collections.Generic;
using System.Linq;

namespace Shuttle.Core.Data
{
	public class DataRepository<T> : IDataRepository<T> where T : class
	{
		private readonly IDatabaseGateway gateway;
		private readonly IDataRowMapper<T> mapper;

		public DataRepository(IDatabaseGateway gateway, IDataRowMapper<T> mapper)
		{
			this.gateway = gateway;
			this.mapper = mapper;
		}

		public IEnumerable<T> FetchAllUsing(DataSource source, IQuery query)
		{
			return gateway.GetRowsUsing(source, query).MappedRowsUsing(mapper).Select(row => row.Result).ToList();
		}

		public T FetchItemUsing(DataSource source, IQuery query)
		{
			var row = gateway.GetSingleRowUsing(source, query);

			return row == null ? default(T) : mapper.Map(row).Result;
		}

		public MappedRow<T> FetchMappedRowUsing(DataSource source, IQuery query)
		{
			var row = gateway.GetSingleRowUsing(source, query);

			return row == null ? null : mapper.Map(row);
		}

		public IEnumerable<MappedRow<T>> FetchMappedRowsUsing(DataSource source, IQuery query)
		{
			return gateway.GetRowsUsing(source, query).MappedRowsUsing(mapper);
		}

		public bool Contains(DataSource source, IQuery query)
		{
			return (gateway.GetScalarUsing<int>(source, query) == 1);
		}
	}
}