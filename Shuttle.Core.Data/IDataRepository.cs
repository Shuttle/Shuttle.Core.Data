using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public interface IDataRepository<T> where T : class
    {
        IEnumerable<T> FetchAllUsing(IDatabaseConnection connection, IQuery query);
		T FetchItemUsing(IDatabaseConnection connection, IQuery query);
		MappedRow<T> FetchMappedRowUsing(IDatabaseConnection connection, IQuery query);
		IEnumerable<MappedRow<T>> FetchMappedRowsUsing(IDatabaseConnection connection, IQuery query);
		bool Contains(IDatabaseConnection connection, IQuery query);
    }
}