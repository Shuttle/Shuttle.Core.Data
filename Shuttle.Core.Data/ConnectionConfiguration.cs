using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ConnectionConfiguration
    {
        public ConnectionConfiguration(string name, string providerName, string connectionString)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            Name = name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string Name { get; }
        public string ProviderName { get; }
        public string ConnectionString { get; }
    }
}