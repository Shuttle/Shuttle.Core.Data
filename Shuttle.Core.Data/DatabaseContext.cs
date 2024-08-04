using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly SemaphoreSlim _lock;
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDbCommandFactory _dbCommandFactory;
        private readonly IDbConnection _dbConnection;
        private bool _disposed;

        public DatabaseContext(string name, string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService, SemaphoreSlim semaphoreSlim)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            ProviderName = Guard.AgainstNullOrEmptyString(providerName, "providerName");

            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            _dbConnection = Guard.AgainstNull(dbConnection, nameof(dbConnection));
            _lock = Guard.AgainstNull(semaphoreSlim, nameof(semaphoreSlim));

            _databaseContextService.Add(this);
        }

        public event EventHandler<TransactionEventArgs> TransactionStarted;
        public event EventHandler<TransactionEventArgs> TransactionCommitted;
        public event EventHandler<TransactionEventArgs> TransactionRolledBack;
        public event EventHandler<EventArgs> Disposed;

        public string Name { get; }
        public DbTransaction Transaction { get; private set; }
        public string ProviderName { get; }

        public DbCommand CreateCommand(IQuery query)
        {
            GuardDisposed();

            var command = _dbCommandFactory.Create(GetOpenConnectionAsync(true).GetAwaiter().GetResult(), Guard.AgainstNull(query, nameof(query)));

            command.Transaction = Transaction;

            return command;
        }

        public IDbConnection GetDbConnection()
        {
            return GetOpenConnectionAsync(true).GetAwaiter().GetResult();
        }

        public bool HasTransaction => Transaction != null;

        public IDatabaseContext BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            GuardDisposed();

            return BeginTransactionAsync(isolationLevel, true).GetAwaiter().GetResult();
        }

        public async Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            GuardDisposed();

            return await BeginTransactionAsync(isolationLevel, false).ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            GuardDisposed();

            CommitTransactionAsync(true).GetAwaiter().GetResult();
        }

        public async Task CommitTransactionAsync()
        {
            await CommitTransactionAsync(false).ConfigureAwait(false);
        }

        public bool IsActive => _databaseContextService.IsActive(this);

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _databaseContextService.Remove(this);

                if (HasTransaction)
                {
                    Transaction.Rollback();

                    TransactionRolledBack?.Invoke(this, new TransactionEventArgs(Transaction));
                }

                _dbConnection.Dispose();
            }
            finally
            {
                _lock.Release();
                _disposed = true;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();

            await new ValueTask();
        }

        private async Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel, bool sync)
        {
            if (HasTransaction || System.Transactions.Transaction.Current != null)
            {
                return this;
            }

            if (sync)
            {
                Transaction = GetOpenConnectionAsync(true).GetAwaiter().GetResult().BeginTransaction(isolationLevel);
            }
            else
            {
                Transaction = await (await GetOpenConnectionAsync(false).ConfigureAwait(false)).BeginTransactionAsync(isolationLevel);
            }

            TransactionStarted?.Invoke(this, new TransactionEventArgs(Transaction));

            return this;
        }

        private async Task CommitTransactionAsync(bool sync)
        {
            if (!HasTransaction)
            {
                return;
            }

            if (sync)
            {
                Transaction.Commit();
            }
            else
            {
                await ((DbTransaction)Transaction).CommitAsync();
            }

            TransactionCommitted?.Invoke(this, new TransactionEventArgs(Transaction));

            Transaction = null;
        }

        private async Task<DbConnection> GetOpenConnectionAsync(bool sync)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                if (sync)
                {
                    ((DbConnection)_dbConnection).Open();
                }
                else
                {
                    await ((DbConnection)_dbConnection).OpenAsync().ConfigureAwait(false);
                }
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
}