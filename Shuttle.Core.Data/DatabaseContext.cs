using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly IDatabaseContextCache _databaseContextCache;
        private readonly IDbCommandFactory _dbCommandFactory;
        private bool _dispose;
        private bool _disposed;
        private readonly IDatabaseContext _activeContext;

        public DatabaseContext(string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextCache = Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            _dispose = true;

            Key = Guid.NewGuid();

            ProviderName = Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Connection = Guard.AgainstNull(dbConnection, nameof(dbConnection));

            if (dbConnection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            _activeContext = databaseContextCache.HasCurrent ? databaseContextCache.Current : null;

            _databaseContextCache.Add(this);
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
            return new DatabaseContext(ProviderName, Connection, _dbCommandFactory, _databaseContextCache)
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

        public IDbCommand CreateCommandToExecute(IQuery query)
        {
            var command = _dbCommandFactory.CreateCommandUsing(Connection, Guard.AgainstNull(query, nameof(query)));
            command.Transaction = Transaction;
            return command;
        }

        public bool HasTransaction => Transaction != null;

        public IDatabaseContext BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public IDatabaseContext BeginTransaction(IsolationLevel isolationLevel)
        {
            if (!HasTransaction && System.Transactions.Transaction.Current == null)
            {
                Transaction = Connection.BeginTransaction(isolationLevel);
            }

            return this;
        }

        public void CommitTransaction()
        {
            if (!HasTransaction)
            {
                return;
            }

            Transaction.Commit();
            Transaction = null;
        }

        public void Dispose()
        {
            _databaseContextCache.Remove(this);

            if (_activeContext != null)
            {
                _databaseContextCache.Use(_activeContext);
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