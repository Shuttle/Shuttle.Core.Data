using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbConnectionConfiguration : IDbConnectionConfiguration
    {
        public DbConnectionConfiguration(DataSource dataSource, string providerName, string connectionString)
        {
            Guard.AgainstNull(dataSource, "dataSource");
            Guard.AgainstNullOrEmptyString(providerName, "providerName");
            Guard.AgainstNullOrEmptyString(connectionString, "connectionString");

            Name = dataSource.Name;
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public string Name { get; private set; }
        public string ProviderName { get; private set; }
        public string ConnectionString { get; private set; }
    }
}