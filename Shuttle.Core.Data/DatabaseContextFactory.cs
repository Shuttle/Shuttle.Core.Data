using System;
using System.Data;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private readonly DataAccessOptions _dataAccessOptions;

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<DataAccessOptions> dataAccessOptions,
            IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            Guard.AgainstNull(dataAccessOptions, nameof(dataAccessOptions));
            
            _connectionStringOptions = Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            _dataAccessOptions = Guard.AgainstNull(dataAccessOptions.Value, nameof(dataAccessOptions.Value));

            DbConnectionFactory = Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            DbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            DatabaseContextCache = Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
        }

        public IDatabaseContext Create(string name)
        {
            var connectionStringOptions = _connectionStringOptions.Get(name);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, name));
            }

            return Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString).WithName(name);
        }

        public IDatabaseContext Create(string providerName, string connectionString)
        {
            return DatabaseContextCache.ContainsConnectionString(connectionString)
                ? DatabaseContextCache.GetConnectionString(connectionString).Suppressed()
                : new DatabaseContext(providerName, DbConnectionFactory.CreateConnection(providerName, connectionString),
                    DbCommandFactory, DatabaseContextCache);
        }

        public IDatabaseContext Create(string providerName, IDbConnection dbConnection)
        {
            Guard.AgainstNull(dbConnection, nameof(dbConnection));

            return DatabaseContextCache.ContainsConnectionString(dbConnection.ConnectionString)
                ? DatabaseContextCache.GetConnectionString(dbConnection.ConnectionString).Suppressed()
                : new DatabaseContext(providerName, dbConnection, DbCommandFactory, DatabaseContextCache);
        }

        public IDbConnectionFactory DbConnectionFactory { get; }
        public IDbCommandFactory DbCommandFactory { get; }
        public IDatabaseContextCache DatabaseContextCache { get; }

        public IDatabaseContext Create()
        {
            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName))
            {
                return Create(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName);
            }

            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultProviderName) && !string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionString))
            {
                return Create(_dataAccessOptions.DatabaseContextFactory.DefaultProviderName, _dataAccessOptions.DatabaseContextFactory.DefaultConnectionString);
            }

            throw new InvalidOperationException(Resources.DatabaseContextFactoryOptionsException);
        }
    }
}