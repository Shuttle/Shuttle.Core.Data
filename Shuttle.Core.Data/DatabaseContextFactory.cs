using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private readonly DataAccessOptions _dataAccessOptions;

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<DataAccessOptions> dataAccessOptions,
            IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
            IDatabaseContextService databaseContextService)
        {
            Guard.AgainstNull(dataAccessOptions, nameof(dataAccessOptions));
            
            _connectionStringOptions = Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            _dataAccessOptions = Guard.AgainstNull(dataAccessOptions.Value, nameof(dataAccessOptions.Value));

            DbConnectionFactory = Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            DbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            DatabaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
        }

        public async Task<IDatabaseContext> Create(string name)
        {
            var connectionStringOptions = _connectionStringOptions.Get(name);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, name));
            }

            var databaseContext = await Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString).ConfigureAwait(false);

            return databaseContext.WithName(name);
        }

        public async Task<IDatabaseContext> Create(string providerName, string connectionString)
        {
            return await Task.FromResult(DatabaseContextService.ContainsConnectionString(connectionString)
                ? DatabaseContextService.GetConnectionString(connectionString).Suppressed()
                : new DatabaseContext(providerName, (DbConnection)DbConnectionFactory.Create(providerName, connectionString),
                    DbCommandFactory, DatabaseContextService)).ConfigureAwait(false);
        }

        public async Task<IDatabaseContext> Create(string providerName, DbConnection dbConnection)
        {
            Guard.AgainstNull(dbConnection, nameof(dbConnection));

            return await Task.FromResult(DatabaseContextService.ContainsConnectionString(dbConnection.ConnectionString)
                ? DatabaseContextService.GetConnectionString(dbConnection.ConnectionString).Suppressed()
                : new DatabaseContext(providerName, dbConnection, DbCommandFactory, DatabaseContextService)).ConfigureAwait(false);
        }

        public IDbConnectionFactory DbConnectionFactory { get; }
        public IDbCommandFactory DbCommandFactory { get; }
        public IDatabaseContextService DatabaseContextService { get; }

        public async Task<IDatabaseContext> Create()
        {
            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName))
            {
                return await Create(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultProviderName) && !string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionString))
            {
                return await Create(_dataAccessOptions.DatabaseContextFactory.DefaultProviderName, _dataAccessOptions.DatabaseContextFactory.DefaultConnectionString).ConfigureAwait(false);
            }

            throw new InvalidOperationException(Resources.DatabaseContextFactoryOptionsException);
        }
    }
}