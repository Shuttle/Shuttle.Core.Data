using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, Action<DataAccessConfigurator> configure = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var configurator = new DataAccessConfigurator(services);

            configure?.Invoke(configurator);

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