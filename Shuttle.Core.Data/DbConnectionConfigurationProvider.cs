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

        public IDbConnectionConfiguration Get(DataSource source)
        {
            Guard.AgainstNull(source, "source");

            if (!dbConnectionConfigurations.ContainsKey(source.Key))
            {
                lock(padlock)
                {
                    if (!dbConnectionConfigurations.ContainsKey(source.Key))
                    {
                        var settings = ConfigurationManager.ConnectionStrings[source.Name];

                        if (settings == null)
                        {
                            throw new InvalidOperationException(
                                string.Format(DataResources.ConnectionStringMissing, source.Name));
                        }

                        dbConnectionConfigurations.Add(source.Key, new DbConnectionConfiguration(source, settings.ProviderName, settings.ConnectionString));
                    }
                }
            }

            return dbConnectionConfigurations[source.Key];
        }
    }
}