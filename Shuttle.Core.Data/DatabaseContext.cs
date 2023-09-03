using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDbCommandFactory _dbCommandFactory;
        private bool _dispose;
        private bool _disposed;
        private readonly IDatabaseContext _activeContext;

        public DatabaseContext(string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService)
        {
            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            _dispose = true;

            Key = Guid.NewGuid();

            ProviderName = Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Connection = Guard.AgainstNull(dbConnection, nameof(dbConnection));

            _activeContext = databaseContextService.HasCurrent ? databaseContextService.Current : null;

            _databaseContextService.Add(this);
        }

        public Guid Key { get; }
        public string Name { get; private set; } = string.Empty;
        public IDbTransaction Transaction { get; private set; }
        public string ProviderName { get; }
        public IDbConnection Connection { get; private set; }

        public IDatabaseContext WithName(string name)
        {
            Name = name;

            return this;
        }

        public IDatabaseContext Suppressed()
        {
            return new DatabaseContext(ProviderName, Connection, _dbCommandFactory, _databaseContextService)
            {
                Transaction = Transaction,
                _dispose = false
            };
        }

        public IDatabaseContext SuppressDispose()
        {
            _dispose = false;

            return this;
        }

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
            if (Connection.State == ConnectionState.Closed)
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

            Transaction = null;
        }

        public void Dispose()
        {
            _databaseContextService.Remove(this);

            if (_activeContext != null && _databaseContextService.Contains(_activeContext))
            {
                _databaseContextService.Use(_activeContext);
            }

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !_dispose)
            {
                return;
            }

            if (disposing)
            {
                if (HasTransaction)
                {
                    Transaction.Rollback();
                }

                Connection.Dispose();
            }

            Connection = null;
            _disposed = true;
        }
    }
}