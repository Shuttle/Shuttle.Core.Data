using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DataAccessBuilder
{
    private DataAccessOptions _dataAccessOptions = new();

    public DataAccessBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public DataAccessOptions Options
    {
        get => _dataAccessOptions;
        set => _dataAccessOptions = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IServiceCollection Services { get; }

    public DataAccessBuilder AddConnectionString(string name, string providerName, string connectionString)
    {
        Guard.AgainstNullOrEmptyString(name);
        Guard.AgainstNullOrEmptyString(providerName);
        Guard.AgainstNullOrEmptyString(connectionString);

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
        Guard.AgainstNullOrEmptyString(name);
        Guard.AgainstNullOrEmptyString(providerName);

        Services.AddOptions<ConnectionStringOptions>(name).Configure<IConfiguration>((option, configuration) =>
        {
            option.ConnectionString = configuration.GetConnectionString(name) ?? string.Empty;
            option.ProviderName = providerName;
            option.Name = name;
        });

        return this;
    }
}