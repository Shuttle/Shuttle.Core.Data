using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbConnectionConfiguration : IDbConnectionConfiguration
    {
        public DbConnectionConfiguration(DataSource source, string providerName, string connectionString)
        {
            Guard.AgainstNull(source, "source");
            Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Guard.AgainstNullOrEmptyString(connectionString, "connectionString");

            Name = source.Name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string Name { get; private set; }
        public string ProviderName { get; private set; }
        public string ConnectionString { get; private set; }
    }
}