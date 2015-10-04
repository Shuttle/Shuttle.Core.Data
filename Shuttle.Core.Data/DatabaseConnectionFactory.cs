using System;
using System.Configuration;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private readonly IDbCommandFactory _dbCommandFactory;
		private readonly IDbConnectionFactory _dbConnectionFactory;

	    private readonly ILog _log;

		public DatabaseConnectionFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory)
		{
			Guard.AgainstNull(dbConnectionFactory, "dbConnectionFactory");
			Guard.AgainstNull(dbCommandFactory, "dbCommandFactory");

			_dbConnectionFactory = dbConnectionFactory;
			_dbCommandFactory = dbCommandFactory;

		    _log = Log.For(this);
		}

		public static IDatabaseConnectionFactory Default()
		{
			return new DatabaseConnectionFactory(new DbConnectionFactory(), new DbCommandFactory());
		}

	    public IDatabaseConnection Create(string connectionStringName)
		{
			var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

			if (settings == null)
			{
				throw new InvalidOperationException(string.Format(DataResources.ConnectionStringMissing, connectionStringName));
			}

			return Create(settings.ProviderName, settings.ConnectionString);
		}

		public IDatabaseConnection Create(string providerName, string connectionString)
		{
			return new DatabaseConnection(_dbConnectionFactory.CreateConnection(providerName, connectionString), _dbCommandFactory);
		}
	}
}