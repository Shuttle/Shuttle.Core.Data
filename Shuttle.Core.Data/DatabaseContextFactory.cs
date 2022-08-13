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
            Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            Guard.AgainstNull(dataAccessOptions, nameof(dataAccessOptions));
            Guard.AgainstNull(dataAccessOptions.Value, nameof(dataAccessOptions.Value));
            Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

            _connectionStringOptions = connectionStringOptions;
            _dataAccessOptions = dataAccessOptions.Value;

            DbConnectionFactory = dbConnectionFactory;
            DbCommandFactory = dbCommandFactory;
            DatabaseContextCache = databaseContextCache;
        }

        public IDatabaseContext Create(string name)
        {
            var connectionStringOptions = _connectionStringOptions.Get(name);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, name));
            }

            return Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString);
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
            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.ConnectionStringName))
            {
                return Create(_dataAccessOptions.DatabaseContextFactory.ConnectionStringName);
            }

            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.ProviderName) && !string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.ConnectionString))
            {
                return Create(_dataAccessOptions.DatabaseContextFactory.ProviderName, _dataAccessOptions.DatabaseContextFactory.ConnectionString);
            }

            throw new InvalidOperationException(Resources.DatabaseContextFactoryOptionsException);
        }
    }
}