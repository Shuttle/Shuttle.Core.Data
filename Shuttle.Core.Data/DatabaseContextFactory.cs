using System;
using System.Configuration;
using System.Data;
using Shuttle.Core.Contract;

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
            Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

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
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissing,
                    connectionStringName));
            }

            return Create(settings.ProviderName, settings.ConnectionString);
        }

        public IDatabaseContext Create(string providerName, string connectionString)
        {
            return DatabaseContextCache.Contains(connectionString)
                ? DatabaseContextCache.Get(connectionString).Suppressed()
                : new DatabaseContext(providerName,
                    DbConnectionFactory.CreateConnection(providerName, connectionString),
                    DbCommandFactory);
        }

        public IDatabaseContext Create(string providerName, IDbConnection dbConnection)
        {
            Guard.AgainstNull(dbConnection, nameof(dbConnection));

            return DatabaseContextCache.Contains(dbConnection.ConnectionString)
                ? DatabaseContextCache.Get(dbConnection.ConnectionString).Suppressed()
                : new DatabaseContext(providerName, dbConnection, DbCommandFactory);
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

#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
        public static IDatabaseContextFactory Default()
        {
            var dbConnectionFactory = new DbConnectionFactory();
#else
        public static IDatabaseContextFactory Default(IDbProviderFactories dbProviderFactories)
        {
            var dbConnectionFactory = new DbConnectionFactory(dbProviderFactories);
#endif

            return new DatabaseContextFactory(dbConnectionFactory, new DbCommandFactory(), new ThreadStaticDatabaseContextCache());
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