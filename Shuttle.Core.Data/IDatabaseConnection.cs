using System;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDatabaseConnection : IDisposable
    {
        IDbTransaction Transaction { get; }
        IDbConnection Connection { get; }
        IDbCommand CreateCommandToExecute(IQuery query);

        bool HasTransaction { get; }

        IDatabaseConnection BeginTransaction();
        void CommitTransaction();
    }
}