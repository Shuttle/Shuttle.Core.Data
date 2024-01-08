using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using IsolationLevel = System.Data.IsolationLevel;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDbCommandFactory _dbCommandFactory;
        private bool _disposed;
        private readonly IDatabaseContext _activeContext;

        public DatabaseContext(string name, string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService)
        {
            Name = Guard.AgainstNullOrEmptyString(name, nameof(name));
            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));

            Key = Guid.NewGuid();

            ProviderName = Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Connection = Guard.AgainstNull(dbConnection, nameof(dbConnection));

            _activeContext = databaseContextService.HasCurrent ? databaseContextService.Current : null;

            _databaseContextService.Add(this);
        }

        public event EventHandler<TransactionEventArgs> TransactionStarted;
        public event EventHandler<TransactionEventArgs> TransactionCommitted;
        public event EventHandler<EventArgs> Disposed;
        public event EventHandler<EventArgs> DisposeIgnored;
        public event EventHandler<TransactionEventArgs> TransactionRolledBack;

        public Guid Key { get; }
        public string Name { get; }
        public int ReferenceCount { get; private set; } = 1;
        public IDbTransaction Transaction { get; private set; }
        public string ProviderName { get; }
        public IDbConnection Connection { get; private set; }

        public IDbCommand CreateCommand(IQuery query)
        {
            return CreateCommandAsync(query, true).GetAwaiter().GetResult();
        }

        public async Task<IDbCommand> CreateCommandAsync(IQuery query)
        {
            return await CreateCommandAsync(query, false).ConfigureAwait(false);
        }

        private async Task<IDbCommand> CreateCommandAsync(IQuery query, bool sync)
        {
            var command = _dbCommandFactory.Create(sync ? GetOpenConnectionAsync(true).GetAwaiter().GetResult() : await GetOpenConnectionAsync(false).ConfigureAwait(false), Guard.AgainstNull(query, nameof(query)));
            
            command.Transaction = Transaction;

            return command;
        }

        private async Task<DbConnection> GetOpenConnectionAsync(bool sync)
        {
            await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);

            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    if (sync)
                    {
                        ((DbConnection)Connection).Open();
                    }
                    else
                    {
                        await ((DbConnection)Connection).OpenAsync().ConfigureAwait(false);
                    }
                }

                return (DbConnection)Connection;
            }
            finally
            {
                _lock.Release();
            }
        }

        public bool HasTransaction => Transaction != null;

        public IDatabaseContext BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return BeginTransactionAsync(isolationLevel, true).GetAwaiter().GetResult();
        }

        public async Task<IDatabaseContext> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            return await BeginTransactionAsync(isolationLevel, false).ConfigureAwait(false);
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

        public void CommitTransaction()
        {
            CommitTransactionAsync(true).GetAwaiter().GetResult();
        }

        public async Task CommitTransactionAsync()
        {
            await CommitTransactionAsync(false).ConfigureAwait(false);
        }

        public IDatabaseContext Referenced()
        {
            ReferenceCount++;

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

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            ReferenceCount--;

            if (ReferenceCount > 0)
            {
                DisposeIgnored?.Invoke(this, EventArgs.Empty);

                return;
            }

            _databaseContextService.Remove(this);

            if (_activeContext != null && _databaseContextService.Contains(_activeContext))
            {
                _databaseContextService.Use(_activeContext);
            }

            if (HasTransaction)
            {
                Transaction.Rollback();

                TransactionRolledBack?.Invoke(this, new TransactionEventArgs(Transaction));
            }

            Connection.Dispose();

            Connection = null;
            Transaction = null;
            _disposed = true;

            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}