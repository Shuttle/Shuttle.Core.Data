using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
	    event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated;

	    Task<IDataReader> GetReader(IQuery query, CancellationToken cancellationToken = default);
		Task<int> Execute(IQuery query, CancellationToken cancellationToken = default);
        Task<T> GetScalar<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<DataTable> GetDataTable(IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<DataRow>> GetRows(IQuery query, CancellationToken cancellationToken = default);
        Task<DataRow> GetRow(IQuery query, CancellationToken cancellationToken = default);
    }
}