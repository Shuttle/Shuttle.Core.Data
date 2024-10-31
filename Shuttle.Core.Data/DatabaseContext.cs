using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DatabaseContext : IDatabaseContext
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IDbCommandFactory _dbCommandFactory;
    private readonly IDbConnection _dbConnection;
    private bool _disposed;
    private readonly IDisposable _databaseContextCollectionRemoval;

    public DatabaseContext(string name, string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService)
    {
        Name = Guard.AgainstNullOrEmptyString(name);
        ProviderName = Guard.AgainstNullOrEmptyString(providerName);

        _dbCommandFactory = Guard.AgainstNull(dbCommandFactory);
        _dbConnection = Guard.AgainstNull(dbConnection);
        _databaseContextService = Guard.AgainstNull(databaseContextService);

        _databaseContextCollectionRemoval = _databaseContextService.Add(this);
    }

    public event EventHandler<TransactionEventArgs>? TransactionStarted;
    public event EventHandler<TransactionEventArgs>? TransactionCommitted;
    public event EventHandler<TransactionEventArgs>? TransactionRolledBack;
    public event EventHandler<EventArgs>? Disposed;

    public bool IsActive => _databaseContextService.IsActive(this);
    public string Name { get; }
    public DbTransaction? Transaction { get; private set; }
    public string ProviderName { get; }

    public async Task<DbCommand> CreateCommandAsync(IQuery query)
    {
        GuardDisposed();

        var command = _dbCommandFactory.Create(await GetOpenConnectionAsync(), Guard.AgainstNull(query));

        command.Transaction = Transaction;

        return command;
    }

    public IDbConnection GetDbConnection()
    {
        return GetOpenConnectionAsync().GetAwaiter().GetResult();
    }

    public bool HasTransaction => Transaction != null;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _databaseContextCollectionRemoval.Dispose();

            if (Transaction != null)
            {
                Transaction.Rollback();

                TransactionRolledBack?.Invoke(this, new(Transaction));
            }

            _dbConnection.Dispose();
        }
        finally
        {
            _disposed = true;
        }

        Disposed?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();

        await new ValueTask();
    }

    public async Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        GuardDisposed();

        if (HasTransaction || System.Transactions.Transaction.Current != null)
        {
            return this;
        }

        Transaction = await (await GetOpenConnectionAsync().ConfigureAwait(false)).BeginTransactionAsync(isolationLevel);

        TransactionStarted?.Invoke(this, new(Transaction));

        return this;
    }

    public async Task CommitTransactionAsync()
    {
        if (Transaction == null)
        {
            return;
        }

        await Transaction.CommitAsync();

        TransactionCommitted?.Invoke(this, new(Transaction));

        Transaction = null;
    }

    private async Task<DbConnection> GetOpenConnectionAsync()
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            await ((DbConnection)_dbConnection).OpenAsync().ConfigureAwait(false);
        }

        return (DbConnection)_dbConnection;
    }

    private void GuardDisposed()
    {
        if (!_disposed)
        {
            return;
        }

        throw new ObjectDisposedException(nameof(DatabaseContext));
    }
}