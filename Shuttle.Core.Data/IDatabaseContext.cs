using System;
using System.Data;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;

namespace Shuttle.Core.Data
{
    public interface IDatabaseContext : IDisposable
    {
	    event EventHandler<TransactionEventArgs> TransactionStarted;
	    event EventHandler<TransactionEventArgs> TransactionCommitted;
	    event EventHandler<EventArgs> Disposed;
	    event EventHandler<EventArgs> DisposeIgnored;

	    Guid Key { get; }
		string Name { get; }
        int ReferenceCount { get; }

        IDbTransaction Transaction { get; }
        IDbConnection Connection { get; }
        IDbCommand CreateCommand(IQuery query);
        Task<IDbCommand> CreateCommandAsync(IQuery query);

        bool HasTransaction { get; }
        string ProviderName { get; }

        IDatabaseContext BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        void CommitTransaction();
        Task CommitTransactionAsync();
	    IDatabaseContext Referenced();
    }
}