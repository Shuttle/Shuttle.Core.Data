using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DbCommandCreatedEventArgs : EventArgs
    {
        public IDbCommand DbCommand { get; }

        public DbCommandCreatedEventArgs(IDbCommand dbCommand)
        {
            DbCommand = Guard.AgainstNull(dbCommand, nameof(dbCommand));
        }
    }
}