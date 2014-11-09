using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		private readonly IDatabaseConnectionCache databaseConnectionCache;
		private readonly IDbCommandFactory dbCommandFactory;
		private readonly IDbConnectionFactory dbConnectionFactory;

	    private readonly ILog log;

		public DatabaseConnectionFactory(IDbConnectionFactory dbConnectionFactory, IDbCommandFactory dbCommandFactory,
		                                 IDatabaseConnectionCache databaseConnectionCache)
		{
			this.dbConnectionFactory = dbConnectionFactory;
			this.databaseConnectionCache = databaseConnectionCache;
			this.dbCommandFactory = dbCommandFactory;

		    log = Log.For(this);
		}

		public static IDatabaseConnectionFactory Default()
		{
			return new DatabaseConnectionFactory(DbConnectionFactory.Default(), new DbCommandFactory(), new ThreadStaticDatabaseConnectionCache());
		}

	    public IDatabaseConnection Create(DataSource source)
		{
			if (databaseConnectionCache.Contains(source))
			{
				var existingDatabaseConnection = new ExistingDatabaseConnection(databaseConnectionCache.Get(source));

				log.Verbose(string.Format(DataResources.ExistingDatabaseConnectionReturned, source.Name));

				return existingDatabaseConnection;
			}

			var databaseConnection = new DatabaseConnection(source, dbConnectionFactory.CreateConnection(source), dbCommandFactory, databaseConnectionCache);

			log.Verbose(string.Format(DataResources.DatabaseConnectionCreated, source.Name));

			return databaseConnection;
		}
	}
}