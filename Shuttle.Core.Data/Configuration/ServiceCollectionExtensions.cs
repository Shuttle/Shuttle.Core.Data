using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, Action<DataAccessBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var options = new DataAccessBuilder(services);

            builder?.Invoke(options);

            services.TryAddSingleton<IValidateOptions<CommandOptions>, CommandOptionsValidator>();
            services.TryAddSingleton<IValidateOptions<ConnectionStringOptions>, ConnectionStringOptionsValidator>();

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