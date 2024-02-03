using System;
using System.Data.Common;
using System.Threading;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private readonly DataAccessOptions _dataAccessOptions;
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDbCommandFactory _dbCommandFactory;

        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public DatabaseContextFactory(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IOptions<DataAccessOptions> dataAccessOptions, IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory, IDatabaseContextService databaseContextService)
        {
            Guard.AgainstNull(dataAccessOptions, nameof(dataAccessOptions));

            _connectionStringOptions = Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            _dataAccessOptions = Guard.AgainstNull(dataAccessOptions.Value, nameof(dataAccessOptions.Value));

            _dbConnectionFactory = Guard.AgainstNull(dbConnectionFactory, nameof(dbConnectionFactory));
            _dbCommandFactory = Guard.AgainstNull(dbCommandFactory, nameof(dbCommandFactory));
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
        }

        public event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;

        public IDatabaseContext Create(string connectionStringName)
        {
            Guard.AgainstNullOrEmptyString(connectionStringName, nameof(connectionStringName));

            _lock.Wait();

            try
            {
                var connectionStringOptions = _connectionStringOptions.Get(connectionStringName);

                if (connectionStringOptions == null || string.IsNullOrEmpty(connectionStringOptions.Name))
                {
                    throw new InvalidOperationException(string.Format(Resources.ConnectionStringMissingException, connectionStringName));
                }

                if (_databaseContextService.Contains(connectionStringName))
                {
                    throw new InvalidOperationException(string.Format(Resources.DuplicateDatabaseContextException, connectionStringName));
                }

                var databaseContext = new DatabaseContext(connectionStringName, connectionStringOptions.ProviderName, (DbConnection)_dbConnectionFactory.Create(connectionStringOptions.ProviderName, connectionStringOptions.ConnectionString), _dbCommandFactory, _databaseContextService);

                DatabaseContextCreated?.Invoke(this, new DatabaseContextEventArgs(databaseContext));

                return databaseContext;
            }
            finally
            {
                _lock.Release();
            }
        }

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