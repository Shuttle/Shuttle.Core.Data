using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        IEnumerable<T> FetchItems(IQuery query);
		T FetchItem(IQuery query);
		MappedRow<T> FetchMappedRow(IQuery query);
		IEnumerable<MappedRow<T>> FetchMappedRows(IQuery query);
		bool Contains(IQuery query);
    }
}