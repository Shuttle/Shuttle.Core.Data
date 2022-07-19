using System;
using System.Data;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private string _connectionString;
        private string _connectionStringName;
        private IDbConnection _dbConnection;
        private string _providerName;

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions,
            IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

            _connectionStringOptions = connectionStringOptions;

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
            if (!string.IsNullOrEmpty(_connectionStringName))
            {
                return Create(_connectionStringName);
            }

            if (!string.IsNullOrEmpty(_providerName) && !string.IsNullOrEmpty(_connectionString))
            {
                return Create(_providerName, _connectionString);
            }

            if (_dbConnection != null)
            {
                return Create(_providerName, _dbConnection);
            }

            throw new InvalidOperationException(string.Format(
                Resources.DatabaseContextFactoryNotConfiguredException, GetType().FullName));
        }

        public IDatabaseContextFactory ConfigureWith(string connectionStringName)
        {
            ClearConfiguredValues();

            _connectionStringName = connectionStringName;

            return this;
        }

        public IDatabaseContextFactory ConfigureWith(string providerName, string connectionString)
        {
            ClearConfiguredValues();

            _providerName = providerName;
            _connectionString = connectionString;

            return this;
        }

        public IDatabaseContextFactory ConfigureWith(IDbConnection dbConnection)
        {
            ClearConfiguredValues();

            _dbConnection = dbConnection;

            return this;
        }

        private void ClearConfiguredValues()
        {
            _connectionStringName = null;
            _providerName = null;
            _connectionString = null;
            _dbConnection = null;
        }
    }
}