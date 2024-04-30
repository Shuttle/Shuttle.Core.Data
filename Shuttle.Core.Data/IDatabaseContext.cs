using System;
using System.Data;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;

namespace Shuttle.Core.Data
{
    public interface IDatabaseContext : IDisposable, IAsyncDisposable
    {
	    event EventHandler<TransactionEventArgs> TransactionStarted;
	    event EventHandler<TransactionEventArgs> TransactionCommitted;
	    event EventHandler<TransactionEventArgs> TransactionRolledBack;
	    event EventHandler<EventArgs> Disposed;

	    Guid Key { get; }
		string Name { get; }

		IDbTransaction Transaction { get; }
		BlockedDbCommand CreateCommand(IQuery query);
		BlockedDbConnection GetDbConnection();

        bool HasTransaction { get; }
        string ProviderName { get; }

        IDatabaseContext BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        void CommitTransaction();
        Task CommitTransactionAsync();
    }
}