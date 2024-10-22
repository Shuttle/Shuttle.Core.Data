using System;
using System.Data.Common;
using System.Threading;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DatabaseContextFactory : IDatabaseContextFactory
{
    private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
    private readonly DataAccessOptions _dataAccessOptions;
    private readonly IDbCommandFactory _dbCommandFactory;

    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<DataAccessOptions> dataAccessOptions, IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory)
    {
        Guard.AgainstNull(dataAccessOptions);

        _connectionStringOptions = Guard.AgainstNull(connectionStringOptions);
        _dataAccessOptions = Guard.AgainstNull(Guard.AgainstNull(dataAccessOptions).Value);

        _dbConnectionFactory = Guard.AgainstNull(dbConnectionFactory);
        _dbCommandFactory = Guard.AgainstNull(dbCommandFactory);
    }

    public event EventHandler<DatabaseContextEventArgs>? DatabaseContextCreated;

    public IDatabaseContext Create(string connectionStringName)
    {
        Guard.AgainstNullOrEmptyString(connectionStringName);

        _lock.Wait();

        try
        {
            var connectionStringOptions = _connectionStringOptions.Get(connectionStringName);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, connectionStringName));
            }

            var databaseContext = new DatabaseContext(connectionStringName, connectionStringOptions.ProviderName, (DbConnection)_dbConnectionFactory.Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString), _dbCommandFactory);

            DatabaseContextCreated?.Invoke(this, new(databaseContext));

            return databaseContext;
        }
        finally
        {
            _lock.Release();
        }
    }

    public IDatabaseContext Create()
    {
        if (string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName))
        {
            throw new InvalidOperationException(Resources.DatabaseContextFactoryOptionsException);
        }

        return Create(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName);
    }
}