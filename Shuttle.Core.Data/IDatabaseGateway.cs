using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
        IDataReader GetReaderUsing(DataSource source, IQuery query);
		int ExecuteUsing(DataSource source, IQuery query);
		T GetScalarUsing<T>(DataSource source, IQuery query);
		DataTable GetDataTableFor(DataSource source, IQuery query);
		IEnumerable<DataRow> GetRowsUsing(DataSource source, IQuery query);
		DataRow GetSingleRowUsing(DataSource source, IQuery query);
    }
}