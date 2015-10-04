using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
        IDataReader GetReaderUsing(IDatabaseConnection connection, IQuery query);
		int ExecuteUsing(IDatabaseConnection connection, IQuery query);
		T GetScalarUsing<T>(IDatabaseConnection connection, IQuery query);
		DataTable GetDataTableFor(IDatabaseConnection connection, IQuery query);
		IEnumerable<DataRow> GetRowsUsing(IDatabaseConnection connection, IQuery query);
		DataRow GetSingleRowUsing(IDatabaseConnection connection, IQuery query);
    }
}