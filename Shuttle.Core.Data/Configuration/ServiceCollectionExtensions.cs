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

            var dataAccessBuilder = new DataAccessBuilder(services);

            builder?.Invoke(dataAccessBuilder);

            services.TryAddSingleton<IValidateOptions<DataAccessOptions>, DataAccessOptionsValidator>();
            services.TryAddSingleton<IValidateOptions<ConnectionStringOptions>, ConnectionStringOptionsValidator>();

            services.AddOptions<DataAccessOptions>().Configure(options =>
            {
                options.CommandTimeout = dataAccessBuilder.Options.CommandTimeout;
                options.DatabaseContextFactory = dataAccessBuilder.Options.DatabaseContextFactory;
            });

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