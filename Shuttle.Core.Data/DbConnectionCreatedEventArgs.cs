using System;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DbConnectionCreatedEventArgs : EventArgs
{
    public DbConnectionCreatedEventArgs(IDbConnection connection)
    {
        Connection = Guard.AgainstNull(connection);
    }

    public IDbConnection Connection { get; }
}