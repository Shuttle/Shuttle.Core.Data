using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
	    event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated;

	    Task<IDataReader> GetReader(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<int> Execute(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<T> GetScalar<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<DataTable> GetDataTable(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<DataRow>> GetRows(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
        Task<DataRow> GetRow(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default);
    }
}