using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
        IDataReader GetReaderUsing(IQuery query);
		int ExecuteUsing(IQuery query);
		T GetScalarUsing<T>(IQuery query);
		DataTable GetDataTableFor(IQuery query);
		IEnumerable<DataRow> GetRowsUsing(IQuery query);
		DataRow GetSingleRowUsing(IQuery query);
    }
}