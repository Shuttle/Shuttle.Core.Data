using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using IsolationLevel = System.Data.IsolationLevel;

namespace Shuttle.Core.Data;

public interface IDatabaseContext : IDisposable, IAsyncDisposable
{
    bool HasTransaction { get; }
    bool IsActive { get; }
    string Name { get; }
    string ProviderName { get; }
    DbTransaction? Transaction { get; }
    Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    Task CommitTransactionAsync();
    Task<DbCommand> CreateCommandAsync(IQuery query);

    event EventHandler<EventArgs> Disposed;

    IDbConnection GetDbConnection();
    event EventHandler<TransactionEventArgs> TransactionCommitted;
    event EventHandler<TransactionEventArgs> TransactionRolledBack;
    event EventHandler<TransactionEventArgs> TransactionStarted;
}