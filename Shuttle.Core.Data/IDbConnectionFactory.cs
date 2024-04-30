using System;
using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDbConnectionFactory
    {
        event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated;

        IDbConnection Create(string providerName, string connectionString);
    }
}