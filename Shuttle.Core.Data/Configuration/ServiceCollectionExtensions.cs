using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, Action<DataAccessOptions> options = null)
        {
            Guard.AgainstNull(services, nameof(services));

            options?.Invoke(new DataAccessOptions(services));

            services.TryAddSingleton<IValidateOptions<CommandSettings>, CommandSettingsValidator>();
            services.TryAddSingleton<IValidateOptions<ConnectionSettings>, ConnectionSettingsValidator>();

            services.TryAddSingleton<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
            services.TryAddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
            services.TryAddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.TryAddSingleton<IDbCommandFactory, DbCommandFactory>();
            services.TryAddSingleton<IDatabaseGateway, DatabaseGateway>();
            services.TryAddSingleton<IDataRowMapper, DataRowMapper>();
            services.TryAddSingleton<IQueryMapper, QueryMapper>();
            services.TryAddSingleton(typeof(IDataRepository<>), typeof(DataRepository<>));

            return services;
        }
    }
}