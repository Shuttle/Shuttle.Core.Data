using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class ExistingDatabaseConnection : IDatabaseConnection
    {
        private readonly IDatabaseConnection _databaseConnection;

        public ExistingDatabaseConnection(IDatabaseConnection databaseConnection)
        {
			Guard.AgainstNull(databaseConnection, "databaseConnection");

            _databaseConnection = databaseConnection;
        }

        public void Dispose()
        {
        }

        public IDbCommand CreateCommandToExecute(IQuery query)
        {
            return _databaseConnection.CreateCommandToExecute(query);
        }

        public bool HasTransaction
        {
            get { return Transaction != null; }
        }

        public IDbTransaction Transaction
        {
            get { return _databaseConnection.Transaction; }
        }

        public IDbConnection Connection
        {
            get { return _databaseConnection.Connection; }
        }

        public IDatabaseConnection BeginTransaction()
        {
            if (!HasTransaction)
            {
                _databaseConnection.BeginTransaction();
            }

            return this;
        }

        public void CommitTransaction()
        {
            if (HasTransaction)
            {
                _databaseConnection.CommitTransaction();
            }
        }
    }
}