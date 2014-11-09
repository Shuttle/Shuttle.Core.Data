using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        IEnumerable<T> FetchAllUsing(DataSource source, IQuery query);
		T FetchItemUsing(DataSource source, IQuery query);
		MappedRow<T> FetchMappedRowUsing(DataSource source, IQuery query);
		IEnumerable<MappedRow<T>> FetchMappedRowsUsing(DataSource source, IQuery query);
		bool Contains(DataSource source, IQuery query);
    }
}