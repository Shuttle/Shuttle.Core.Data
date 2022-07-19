﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataAccessBuilder
    {
        public const string SectionName = "Shuttle:DataAccess";

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
                var connectionString = configuration.GetConnectionString(name);
                
                Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

                option.ConnectionString = connectionString;
                option.ProviderName = providerName;
                option.Name = name;
            });

            return this;
        }

        public DataAccessBuilder WithCommandTimeout(int timeout)
        {
            Services.Configure<CommandOptions>(option =>
            {
                option.CommandTimeout = timeout;
            });

            return this;
        }

        public DataAccessBuilder GetCommandTimeout(string key = null)
        {
            Services.AddOptions<CommandOptions>().Configure<IConfiguration>((option, configuration) =>
            {
                var settings = configuration.GetRequiredSection(key ?? SectionName).Get<CommandOptions>();

                option.CommandTimeout = settings.CommandTimeout;
            });

            return this;
        }
    }
}