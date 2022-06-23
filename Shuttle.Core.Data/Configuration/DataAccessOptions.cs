using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataAccessOptions
    {
        public const string SectionName = "Shuttle:DataAccess";
        
        private readonly IServiceCollection _services;

        public DataAccessOptions(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            _services = services;

            _services.AddOptions<CommandSettings>().Configure<IConfiguration>((option, configuration) =>
            {
                var settings = configuration.GetSection(SectionName).Get<CommandSettings>();

                option.CommandTimeout = settings?.CommandTimeout ?? 0;
            });
        }

        public DataAccessOptions AddConnection(string name, string providerName, string connectionString)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            _services.Configure<ConnectionSettings>(name, option =>
            {
                option.ConnectionString = connectionString;
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }

        public DataAccessOptions AddConnectionString(string name, string providerName)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
            
            _services.AddOptions<ConnectionSettings>(name).Configure<IConfiguration>((option, configuration) =>
            {
                var connectionString = configuration.GetConnectionString(name);
                
                Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

                option.ConnectionString = connectionString;
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }

        public DataAccessOptions AddCommandTimeout(int timeout)
        {
            _services.Configure<CommandSettings>(option =>
            {
                option.CommandTimeout = timeout;
            });

            return this;
        }
    }
}