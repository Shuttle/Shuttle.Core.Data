using System.Data;

namespace Shuttle.Core.Data
{
    public class ExistingDatabaseConnection : IDatabaseConnection
    {
        private readonly IDatabaseConnection databaseConnection;

        public ExistingDatabaseConnection(IDatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void Dispose()
        {
        }

        public IDbCommand CreateCommandToExecute(IQuery query)
        {
            return databaseConnection.CreateCommandToExecute(query);
        }

        public bool HasTransaction
        {
            get { return Transaction != null; }
        }

        public IDbTransaction Transaction
        {
            get { return databaseConnection.Transaction; }
        }

        public IDbConnection Connection
        {
            get { return databaseConnection.Connection; }
        }

        public IDatabaseConnection BeginTransaction()
        {
            if (!HasTransaction)
            {
                databaseConnection.BeginTransaction();
            }

            return this;
        }

        public void CommitTransaction()
        {
            if (HasTransaction)
            {
                databaseConnection.CommitTransaction();
            }
        }
    }
}