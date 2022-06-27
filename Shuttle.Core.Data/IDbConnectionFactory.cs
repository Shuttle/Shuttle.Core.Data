using System;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDbConnectionFactory 
    {
        event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated;

        IDbConnection CreateConnection(string providerName, string connectionString);
    }
}