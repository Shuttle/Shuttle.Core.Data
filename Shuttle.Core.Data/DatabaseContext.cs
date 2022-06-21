using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly IDatabaseContextCache _databaseContextCache;
        private readonly ILogger<DatabaseContext> _logger;
        private readonly IDbCommandFactory _dbCommandFactory;
        private bool _dispose;
        private bool _disposed;

        public DatabaseContext(ILogger<DatabaseContext> logger, string providerName, IDbConnection dbConnection, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            Guard.AgainstNull(logger, nameof(logger));
            Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Guard.AgainstNull(dbConnection, nameof(dbConnection));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));

            _logger = logger;
            _dbCommandFactory = dbCommandFactory;
            _databaseContextCache = databaseContextCache;
            _dispose = true;

            Key = Guid.NewGuid();

            ProviderName = providerName;
            Connection = dbConnection;

            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    Connection.Open();

                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace(string.Format(Resources.VerboseDbConnectionOpened, dbConnection.Database));
                    }
                }
                else
                {
                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace(string.Format(Resources.VerboseDbConnectionAlreadyOpen, dbConnection.Database));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format(Resources.DbConnectionOpenException, dbConnection.Database, ex.Message));

                throw;
            }

            _databaseContextCache.Add(this);
        }

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
            return new DatabaseContext(_logger, ProviderName, Connection, _dbCommandFactory, _databaseContextCache)
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
            _databaseContextCache.Remove(this);

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