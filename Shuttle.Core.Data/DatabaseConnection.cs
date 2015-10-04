using System;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly IDbCommandFactory _dbCommandFactory;
        private readonly ILog _log;

        private bool disposed;

        public DatabaseConnection(IDbConnection dbConnection, IDbCommandFactory dbCommandFactory)
        {
			Guard.AgainstNull(dbConnection, "dbConnection");
			Guard.AgainstNull(dbCommandFactory, "dbCommandFactory");

            _dbCommandFactory = dbCommandFactory;

            Connection = dbConnection;

            _log = Log.For(this);

            try
            {
	            if (dbConnection.State == ConnectionState.Closed)
	            {
		            Connection.Open();

					_log.Verbose(string.Format(DataResources.VerboseDbConnectionOpened, dbConnection.Database));
				}
	            else
	            {
					_log.Verbose(string.Format(DataResources.VerboseDbConnectionAlreadyOpen, dbConnection.Database));
	            }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format(DataResources.DbConnectionOpenException, dbConnection.Database, ex.Message));

                throw;
            }
        }

        public IDbCommand CreateCommandToExecute(IQuery query)
        {
            var command = _dbCommandFactory.CreateCommandUsing(Connection, query);
            command.Transaction = Transaction;
            return command;
        }

        public bool HasTransaction
        {
            get { return Transaction != null; }
        }

        public IDbTransaction Transaction { get; private set; }
        public IDbConnection Connection { get; private set; }

        public IDatabaseConnection BeginTransaction()
        {
            if (!HasTransaction && System.Transactions.Transaction.Current == null)
            {
                Transaction = Connection.BeginTransaction();
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
            if (disposed)
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
            disposed = true;
        }
    }
}