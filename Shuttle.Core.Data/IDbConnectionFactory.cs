using System;
using System.Data;

namespace Shuttle.Core.Data;

public interface IDbConnectionFactory
{
    IDbConnection Create(string providerName, string connectionString);
    event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated;
}