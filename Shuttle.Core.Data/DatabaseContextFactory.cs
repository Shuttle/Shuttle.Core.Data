using System;
using System.Configuration;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private string _connectionString;
        private string _connectionStringName;
        private IDbConnection _dbConnection;
        private string _providerName;

        public DatabaseContextFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
            IDatabaseContextCache databaseContextCache)
        {
            Guard.AgainstNull(dbConnectionFactory, "dbConnectionFactory");
            Guard.AgainstNull(dbCommandFactory, "dbCommandFactory");
            Guard.AgainstNull(databaseContextCache, "databaseContextCache");

            DbConnectionFactory = dbConnectionFactory;
            DbCommandFactory = dbCommandFactory;
            DatabaseContextCache = databaseContextCache;

            DatabaseContext.Assign(databaseContextCache);
        }

        public IDatabaseContext Create(string connectionStringName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
            {
                throw new InvalidOperationException(string.Format(DataResources.ConnectionStringMissing,
                    connectionStringName));
            }

            return Create(settings.ProviderName, settings.ConnectionString);
        }

        public IDatabaseContext Create(string providerName, string connectionString)
        {
            return DatabaseContextCache.Contains(connectionString)
                ? DatabaseContextCache.Get(connectionString).Suppressed()
                : new DatabaseContext(DbConnectionFactory.CreateConnection(providerName, connectionString),
                    DbCommandFactory);
        }

        public IDatabaseContext Create(IDbConnection dbConnection)
        {
            Guard.AgainstNull(dbConnection, "dbConnection");

            return DatabaseContextCache.Contains(dbConnection.ConnectionString)
                ? DatabaseContextCache.Get(dbConnection.ConnectionString).Suppressed()
                : new DatabaseContext(dbConnection, DbCommandFactory);
        }

        public IDbConnectionFactory DbConnectionFactory { get; private set; }
        public IDbCommandFactory DbCommandFactory { get; private set; }
        public IDatabaseContextCache DatabaseContextCache { get; private set; }

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
                return Create(_dbConnection);
            }

            throw new InvalidOperationException(string.Format(
                DataResources.DatabaseContextFactoryNotConfiguredException, GetType().FullName));
        }

        public void ConfigureWith(string connectionStringName)
        {
            ClearConfiguredValues();

            _connectionStringName = connectionStringName;
        }

        public void ConfigureWith(string providerName, string connectionString)
        {
            ClearConfiguredValues();

            _providerName = providerName;
            _connectionString = connectionString;
        }

        public void ConfigureWith(IDbConnection dbConnection)
        {
            ClearConfiguredValues();

            _dbConnection = dbConnection;
        }

        public static IDatabaseContextFactory Default()
        {
            return new DatabaseContextFactory(new DbConnectionFactory(), new DbCommandFactory(),
                new ThreadStaticDatabaseContextCache());
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