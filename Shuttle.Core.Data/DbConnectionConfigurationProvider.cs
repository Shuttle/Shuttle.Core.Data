using System;
using System.Collections.Generic;
using System.Configuration;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbConnectionConfigurationProvider : IDbConnectionConfigurationProvider
    {
        private static readonly object padlock = new object();

        private readonly Dictionary<string, IDbConnectionConfiguration> dbConnectionConfigurations =
            new Dictionary<string, IDbConnectionConfiguration>();

        public IDbConnectionConfiguration Get(DataSource dataSource)
        {
            Guard.AgainstNull(dataSource, "dataSource");

            if (!dbConnectionConfigurations.ContainsKey(dataSource.Key))
            {
                lock(padlock)
                {
                    if (!dbConnectionConfigurations.ContainsKey(dataSource.Key))
                    {
                        var settings = ConfigurationManager.ConnectionStrings[dataSource.Name];

                        if (settings == null)
                        {
                            throw new InvalidOperationException(
                                string.Format(DataResources.ConnectionStringMissing, dataSource.Name));
                        }

                        dbConnectionConfigurations.Add(dataSource.Key, new DbConnectionConfiguration(dataSource, settings.ProviderName, settings.ConnectionString));
                    }
                }
            }

            return dbConnectionConfigurations[dataSource.Key];
        }
    }
}