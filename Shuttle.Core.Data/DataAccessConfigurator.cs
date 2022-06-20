using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataAccessConfigurator
    {
        private readonly IServiceCollection _services;

        public DataAccessConfigurator(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            _services = services;
        }

        public DataAccessConfigurator AddConnection(string name, string providerName, string connectionString)
        {
            _services.Configure<ConnectionSettings>(name, option =>
            {
                option.ConnectionString = connectionString;
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }
    }
}