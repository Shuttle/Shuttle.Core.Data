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

	    IDataReader GetReader(IQuery query);
	    int Execute(IQuery query);
	    T GetScalar<T>(IQuery query);
	    DataTable GetDataTable(IQuery query);
	    IEnumerable<DataRow> GetRows(IQuery query);
	    DataRow GetRow(IQuery query);
	    
	    Task<IDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken = default);
        Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<DataRow>> GetRowsAsync(IQuery query, CancellationToken cancellationToken = default);
        Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken = default);
    }
}