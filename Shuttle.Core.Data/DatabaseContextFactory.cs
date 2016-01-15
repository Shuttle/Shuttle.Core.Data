using System;
using System.Configuration;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseContextFactory : IDatabaseContextFactory, IConfiguredDatabaseContextFactory
	{
		private readonly IDbCommandFactory _dbCommandFactory;
		private readonly IDatabaseContextCache _databaseContextCache;
		private readonly IDbConnectionFactory _dbConnectionFactory;

		private string _connectionStringName = null;
		private string _providerName = null;
		private string _connectionString = null;
		private IDbConnection _dbConnection = null;

		public DatabaseContextFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory, IDatabaseContextCache databaseContextCache)
		{
			Guard.AgainstNull(dbConnectionFactory, "dbConnectionFactory");
			Guard.AgainstNull(dbCommandFactory, "dbCommandFactory");
			Guard.AgainstNull(databaseContextCache, "databaseContextCache");

			_dbConnectionFactory = dbConnectionFactory;
			_dbCommandFactory = dbCommandFactory;
			_databaseContextCache = databaseContextCache;

			DatabaseContext.Assign(databaseContextCache);
		}

		public static IDatabaseContextFactory Default()
		{
			return new DatabaseContextFactory(new DbConnectionFactory(), new DbCommandFactory(),
				new ThreadStaticDatabaseContextCache());
		}

		public IDatabaseContext Create(string connectionStringName)
		{
			var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

			if (settings == null)
			{
				throw new InvalidOperationException(string.Format(DataResources.ConnectionStringMissing, connectionStringName));
			}

			return Create(settings.ProviderName, settings.ConnectionString);
		}

		public IDatabaseContext Create(string providerName, string connectionString)
		{
			return _databaseContextCache.Contains(connectionString)
				? _databaseContextCache.Get(connectionString).Suppressed()
				: new DatabaseContext(_dbConnectionFactory.CreateConnection(providerName, connectionString), _dbCommandFactory);
		}

		public IDatabaseContext Create(IDbConnection dbConnection)
		{
			Guard.AgainstNull(dbConnection, "dbConnection");

			return _databaseContextCache.Contains(dbConnection.ConnectionString)
				? _databaseContextCache.Get(dbConnection.ConnectionString).Suppressed()
				: new DatabaseContext(dbConnection, _dbCommandFactory);
		}

		public IDatabaseContext Create()
		{
			if (!string.IsNullOrEmpty(_connectionStringName))
			{
				return Create(_connectionStringName);
			}

			if (!string.IsNullOrEmpty(_providerName) && !string.IsNullOrEmpty(_connectionString))
			{
				return Create(_providerName, _connectionString);
			}

			if (_dbConnection != null)
			{
				return Create(_dbConnection);
			}

			throw new InvalidOperationException(string.Format(DataResources.DatabaseContextFactoryNotConfiguredException, GetType().FullName));
		}

		public void ConfigureWith(string connectionStringName)
		{
			ClearConfiguredValues();

			_connectionStringName = connectionStringName;
		}

		private void ClearConfiguredValues()
		{
			_connectionStringName = null;
			_providerName = null;
			_connectionString = null;
			_dbConnection = null;
		}

		public void ConfigureWith(string providerName, string connectionString)
		{
			ClearConfiguredValues();

			_providerName = providerName;
			_connectionString = connectionString;
		}

		public void ConfigureWith(IDbConnection dbConnection)
		{
			ClearConfiguredValues();

			_dbConnection = dbConnection;
		}
	}
}