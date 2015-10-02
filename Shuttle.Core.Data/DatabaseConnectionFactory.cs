using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private readonly IDatabaseConnectionCache _databaseConnectionCache;
		private readonly IDbCommandFactory _dbCommandFactory;
		private readonly IDbConnectionFactory _dbConnectionFactory;

	    private readonly ILog _log;

		public DatabaseConnectionFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
		                                 IDatabaseConnectionCache databaseConnectionCache)
		{
			Guard.AgainstNull(dbConnectionFactory, "dbConnectionFactory");
			Guard.AgainstNull(dbCommandFactory, "dbCommandFactory");
			Guard.AgainstNull(databaseConnectionCache, "databaseConnectionCache");

			_dbConnectionFactory = dbConnectionFactory;
			_databaseConnectionCache = databaseConnectionCache;
			_dbCommandFactory = dbCommandFactory;

		    _log = Log.For(this);
		}

		public static IDatabaseConnectionFactory Default()
		{
			return new DatabaseConnectionFactory(DbConnectionFactory.Default(), new DbCommandFactory(), new ThreadStaticDatabaseConnectionCache());
		}

	    public IDatabaseConnection Create(DataSource source)
		{
			if (_databaseConnectionCache.Contains(source))
			{
				var existingDatabaseConnection = new ExistingDatabaseConnection(_databaseConnectionCache.Get(source));

				_log.Verbose(string.Format(DataResources.ExistingDatabaseConnectionReturned, source.Name));

				return existingDatabaseConnection;
			}

			var databaseConnection = new DatabaseConnection(source, _dbConnectionFactory.CreateConnection(source), _dbCommandFactory, _databaseConnectionCache);

			_log.Verbose(string.Format(DataResources.DatabaseConnectionCreated, source.Name));

			return databaseConnection;
		}
	}
}