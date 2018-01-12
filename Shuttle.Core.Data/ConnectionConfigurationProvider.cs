using System;
using System.Configuration;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ConnectionConfigurationProvider : IConnectionConfigurationProvider
    {
        public ConnectionConfiguration Get(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            var settings = ConfigurationManager.ConnectionStrings[name];

            if (settings == null)
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissing, name));
            }

            return new ConnectionConfiguration(name, settings.ProviderName, settings.ConnectionString);
        }
    }
}