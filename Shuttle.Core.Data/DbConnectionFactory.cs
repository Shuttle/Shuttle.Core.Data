using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private static readonly object padlock = new object();

        private readonly IDbConnectionConfigurationProvider dbConnectionConfigurationProvider;

        private readonly Dictionary<string, DbProviderFactory> dbProviderFactories =
            new Dictionary<string, DbProviderFactory>();

        private readonly ILog log;

        public DbConnectionFactory(IDbConnectionConfigurationProvider dbConnectionConfigurationProvider)
        {
            this.dbConnectionConfigurationProvider = dbConnectionConfigurationProvider;

            log = Log.For(this);
        }

        public static IDbConnectionFactory Default()
        {
			return new DbConnectionFactory(new DbConnectionConfigurationProvider());
        }

        public IDbConnection CreateConnection(DataSource source)
        {
            var dbConnectionConfiguration = dbConnectionConfigurationProvider.Get(source);

            if (!dbProviderFactories.ContainsKey(source.Key))
            {
                lock(padlock)
                {
                    if (!dbProviderFactories.ContainsKey(source.Key))
                    {
                        var factory = DbProviderFactories.GetFactory(dbConnectionConfiguration.ProviderName);

                        dbProviderFactories.Add(source.Key, factory);

						log.Verbose(string.Format(DataResources.DbProviderFactoryCached, dbConnectionConfiguration.ProviderName));
                    }
                }
            }

            var dbProviderFactory = dbProviderFactories[source.Key];

            var connection = dbProviderFactory.CreateConnection();

	        connection.ConnectionString = dbConnectionConfiguration.ConnectionString;

			log.Verbose(string.Format(DataResources.DbConnectionCreated, source.Name));

            return connection;
        }
    }
}