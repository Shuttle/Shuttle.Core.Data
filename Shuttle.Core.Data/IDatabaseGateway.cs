using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data
{
    public interface IDatabaseGateway
    {
	    BlockedDbDataReader GetReader(IQuery query, CancellationToken cancellationToken = default);
	    int Execute(IQuery query, CancellationToken cancellationToken = default);
	    T GetScalar<T>(IQuery query, CancellationToken cancellationToken = default);
	    DataTable GetDataTable(IQuery query, CancellationToken cancellationToken = default);
	    IEnumerable<DataRow> GetRows(IQuery query, CancellationToken cancellationToken = default);
	    DataRow GetRow(IQuery query, CancellationToken cancellationToken = default);
	    
	    Task<BlockedDbDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken = default);
        Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken = default);
        Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken = default);
		Task<IEnumerable<DataRow>> GetRowsAsync(IQuery query, CancellationToken cancellationToken = default);
        Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken = default);
    }
}