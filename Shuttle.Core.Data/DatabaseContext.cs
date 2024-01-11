using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;

        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDbCommandFactory _dbCommandFactory;
        private readonly SemaphoreSlim _dbCommandLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _dbDataReaderLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _dbConnectionLock = new SemaphoreSlim(1, 1);
        private readonly IDbConnection _dbConnection;

        public DatabaseContext(string name, string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService, SemaphoreSlim semaphore)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            _semaphore= Guard.AgainstNull(semaphore, nameof(semaphore));

            Key = Guid.NewGuid();

            ProviderName = Guard.AgainstNullOrEmptyString(providerName, "providerName");
            _dbConnection = Guard.AgainstNull(dbConnection, nameof(dbConnection));

            _databaseContextService.Add(this);
        }

        public event EventHandler<TransactionEventArgs> TransactionStarted;
        public event EventHandler<TransactionEventArgs> TransactionCommitted;
        public event EventHandler<TransactionEventArgs> TransactionRolledBack;
        public event EventHandler<EventArgs> Disposed;

        public Guid Key { get; }
        public string Name { get; }
        public IDbTransaction Transaction { get; private set; }
        public string ProviderName { get; }

        private void GuardDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(nameof(DatabaseContext));
        }

        public BlockedDbCommand CreateCommand(IQuery query)
        {
            GuardDisposed();

            _dbCommandLock.Wait(CancellationToken.None);

            var command = _dbCommandFactory.Create(GetOpenConnectionAsync(true).GetAwaiter().GetResult(), Guard.AgainstNull(query, nameof(query)));

            command.Transaction = Transaction;

            return new BlockedDbCommand((DbCommand)command, new BlockingSemaphoreSlim(_dbCommandLock), _dbDataReaderLock);
        }

        public BlockedDbConnection GetBlockedDbConnection()
        {
            _dbConnectionLock.Wait(CancellationToken.None);

            return new BlockedDbConnection(GetOpenConnectionAsync(true).GetAwaiter().GetResult(), new BlockingSemaphoreSlim(_dbConnectionLock));
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

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _semaphore.Wait();

            try
            {
                _databaseContextService.Remove(this);

                if (HasTransaction)
                {
                    Transaction.Rollback();

                    TransactionRolledBack?.Invoke(this, new TransactionEventArgs(Transaction));
                }

                _dbConnection.Dispose();
                _disposed = true;
            }
            finally
            {
                _semaphore.Release();
            }

            Disposed?.Invoke(this, EventArgs.Empty);
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
    }
}