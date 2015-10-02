using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private static readonly object padlock = new object();

        private readonly IDbConnectionConfigurationProvider _dbConnectionConfigurationProvider;

        private readonly Dictionary<string, DbProviderFactory> _dbProviderFactories =
            new Dictionary<string, DbProviderFactory>();

        private readonly ILog _log;

        public DbConnectionFactory(IDbConnectionConfigurationProvider dbConnectionConfigurationProvider)
        {
			Guard.AgainstNull(dbConnectionConfigurationProvider, "dbConnectionConfigurationProvider");

            _dbConnectionConfigurationProvider = dbConnectionConfigurationProvider;

            _log = Log.For(this);
        }

        public static IDbConnectionFactory Default()
        {
			return new DbConnectionFactory(new DbConnectionConfigurationProvider());
        }

        public IDbConnection CreateConnection(DataSource source)
        {
            var dbConnectionConfiguration = _dbConnectionConfigurationProvider.Get(source);

            if (!_dbProviderFactories.ContainsKey(source.Key))
            {
                lock(padlock)
                {
                    if (!_dbProviderFactories.ContainsKey(source.Key))
                    {
                        var factory = DbProviderFactories.GetFactory(dbConnectionConfiguration.ProviderName);

                        _dbProviderFactories.Add(source.Key, factory);

						_log.Verbose(string.Format(DataResources.DbProviderFactoryCached, dbConnectionConfiguration.ProviderName));
                    }
                }
            }

            var dbProviderFactory = _dbProviderFactories[source.Key];

            var connection = dbProviderFactory.CreateConnection();

	        connection.ConnectionString = dbConnectionConfiguration.ConnectionString;

			_log.Verbose(string.Format(DataResources.DbConnectionCreated, source.Name));

            return connection;
        }
    }
}