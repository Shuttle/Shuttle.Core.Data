using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DbCommandCreatedEventArgs : EventArgs
{
    public DbCommandCreatedEventArgs(IDbCommand dbCommand)
    {
        DbCommand = Guard.AgainstNull(dbCommand);
    }

    public IDbCommand DbCommand { get; }
}