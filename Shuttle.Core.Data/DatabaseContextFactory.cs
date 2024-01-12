using System;
using System.Data.Common;
using System.Threading;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1,1);
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

        public IDatabaseContext Create(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            _lock.Wait();

            try
            {
                var connectionStringOptions = _connectionStringOptions.Get(name);

                if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
                {
                    throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, name));
                }

                if (DatabaseContextService.Contains(name))
                {
                    throw new InvalidOperationException(string.Format(Resources.DuplicateDatabaseContextException, name));
                }

                var databaseContext = new DatabaseContext(name, connectionStringOptions.ProviderName, (DbConnection)DbConnectionFactory.Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString), DbCommandFactory, DatabaseContextService);

                DatabaseContextCreated?.Invoke(this, new DatabaseContextEventArgs(databaseContext));

                return databaseContext;
            }
            finally
            {
                _lock.Release();
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