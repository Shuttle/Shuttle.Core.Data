using System;
using System.Data;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private static IDatabaseContextCache _databaseContextCache;
        private readonly IDbCommandFactory _dbCommandFactory;
        private bool _dispose;
        private bool _disposed;

        public DatabaseContext(string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory)
        {
            Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Guard.AgainstNull(dbConnection, nameof(dbConnection));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));

            _dbCommandFactory = dbCommandFactory;
            _dispose = true;

            Key = Guid.NewGuid();

            ProviderName = providerName;
            Connection = dbConnection;

            var log = Log.For(this);

            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    Connection.Open();

                    log.Verbose(string.Format(Resources.VerboseDbConnectionOpened, dbConnection.Database));
                }
                else
                {
                    log.Verbose(string.Format(Resources.VerboseDbConnectionAlreadyOpen, dbConnection.Database));
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format(Resources.DbConnectionOpenException, dbConnection.Database, ex.Message));

                throw;
            }

            GuardedDatabaseContextStore().Add(this);
        }

        public static IDatabaseContext Current => GuardedDatabaseContextStore().Current;

        public Guid Key { get; }
        public string Name { get; private set; }
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
            return new DatabaseContext(ProviderName, Connection, _dbCommandFactory)
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
            var command = _dbCommandFactory.CreateCommandUsing(Connection, query);
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
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            GuardedDatabaseContextStore().Remove(this);

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

        public static void Assign(IDatabaseContextCache cache)
        {
            Guard.AgainstNull(cache, nameof(cache));

            _databaseContextCache = cache;
        }

        private static IDatabaseContextCache GuardedDatabaseContextStore()
        {
            if (_databaseContextCache == null)
            {
                throw new Exception(Resources.DatabaseContextCacheMissingException);
            }

            return _databaseContextCache;
        }
    }
}