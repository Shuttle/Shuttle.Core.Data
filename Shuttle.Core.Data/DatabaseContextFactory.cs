using System;
using System.Data.Common;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private readonly DataAccessOptions _dataAccessOptions;

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<DataAccessOptions> dataAccessOptions, IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService)
        {
            Guard.AgainstNull(dataAccessOptions, nameof(dataAccessOptions));
            
            _connectionStringOptions = Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            _dataAccessOptions = Guard.AgainstNull(dataAccessOptions.Value, nameof(dataAccessOptions.Value));

            DbConnectionFactory = Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            DbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            DatabaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
        }

        public event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;
        public event EventHandler<DatabaseContextEventArgs> DatabaseContextSuppressed;

        public IDatabaseContext Create(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            var connectionStringOptions = _connectionStringOptions.Get(name);

            if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, name));
            }

            if (DatabaseContextService.Contains(name))
            {
                var databaseContext = DatabaseContextService.Get(name).Suppressed();

                DatabaseContextSuppressed?.Invoke(this, new DatabaseContextEventArgs(databaseContext));

                return databaseContext;
            }
            else
            {
                var databaseContext = new DatabaseContext(name, connectionStringOptions.ProviderName, (DbConnection)DbConnectionFactory.Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString), DbCommandFactory, DatabaseContextService);

                DatabaseContextCreated?.Invoke(this, new DatabaseContextEventArgs(databaseContext));

                return databaseContext;
            }
        }

        public IDbConnectionFactory DbConnectionFactory { get; }
        public IDbCommandFactory DbCommandFactory { get; }
        public IDatabaseContextService DatabaseContextService { get; }

        public IDatabaseContext Create()
        {
            if (string.IsNullOrEmpty(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName))
            {
                throw new InvalidOperationException(Resources.DatabaseContextFactoryOptionsException);
            }

            return Create(_dataAccessOptions.DatabaseContextFactory.DefaultConnectionStringName);
        }
    }
}