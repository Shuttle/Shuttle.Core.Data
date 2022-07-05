using System;
using System.Data;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringSettings> _connectionSettings;
        private string _connectionString;
        private string _connectionStringName;
        private IDbConnection _dbConnection;
        private string _providerName;

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringSettings> connectionSettings,
            IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            Guard.AgainstNull(connectionSettings, nameof(connectionSettings));
            Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

            _connectionSettings = connectionSettings;

            DbConnectionFactory = dbConnectionFactory;
            DbCommandFactory = dbCommandFactory;
            DatabaseContextCache = databaseContextCache;
        }

        public IDatabaseContext Create(string name)
        {
            var connectionSettings = _connectionSettings.Get(name);

            if (connectionSettings == null || string.IsNullOrEmpty(connectionSettings.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionSettingsMissing, name));
            }

            return Create(connectionSettings.ProviderName, connectionSettings.ConnectionString);
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