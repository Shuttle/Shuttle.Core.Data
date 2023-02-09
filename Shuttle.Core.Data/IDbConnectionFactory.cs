using System;
using System.Data;
using System.Data.Common;

namespace Shuttle.Core.Data
{
    public interface IDbConnectionFactory 
    {
        event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated;

        DbConnection Create(string providerName, string connectionString);
    }
}