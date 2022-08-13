using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataAccessBuilder
    {
        public const string SectionName = "Shuttle:DataAccess";

        private DataAccessOptions _dataAccessOptions = new DataAccessOptions();

        public DataAccessOptions Options
        {
            get => _dataAccessOptions;
            set => _dataAccessOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IServiceCollection Services { get; }

        public DataAccessBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public DataAccessBuilder AddConnectionString(string name, string providerName, string connectionString)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            Services.Configure<ConnectionStringOptions>(name, option =>
            {
                option.ConnectionString = connectionString;
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }

        public DataAccessBuilder AddConnectionString(string name, string providerName)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
            
            Services.AddOptions<ConnectionStringOptions>(name).Configure<IConfiguration>((option, configuration) =>
            {
                option.ConnectionString = configuration.GetConnectionString(name);
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }
    }
}