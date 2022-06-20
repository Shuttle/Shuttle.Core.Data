using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
        IDataReader GetReader(IQuery query);
		int Execute(IQuery query);
		T GetScalar<T>(IQuery query);
		DataTable GetDataTable(IQuery query);
		IEnumerable<DataRow> GetRows(IQuery query);
		DataRow GetRow(IQuery query);
    }
}