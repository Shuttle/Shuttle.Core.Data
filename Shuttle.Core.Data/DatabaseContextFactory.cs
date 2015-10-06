using System;
using System.Configuration;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseContextFactory : IDatabaseContextFactory
	{
		private readonly IDbCommandFactory _dbCommandFactory;
		private readonly IDatabaseContextCache _databaseContextCache;
		private readonly IDbConnectionFactory _dbConnectionFactory;

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
	}
}