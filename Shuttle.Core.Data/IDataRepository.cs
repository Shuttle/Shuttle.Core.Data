using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        IEnumerable<T> FetchAllUsing(IQuery query);
		T FetchItemUsing(IQuery query);
		MappedRow<T> FetchMappedRowUsing(IQuery query);
		IEnumerable<MappedRow<T>> FetchMappedRowsUsing(IQuery query);
		bool Contains(IQuery query);
    }
}