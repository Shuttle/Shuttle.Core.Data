using System.Data;
using System.Data.Common;
using Shuttle.Core.Logging;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
	    private readonly ILog _log;

	    public DbConnectionFactory()
	    {
	        _log = Log.For(this);
	    }

        public IDbConnection CreateConnection(string providerName, string connectionString)
		{
		    var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            var connection = dbProviderFactory.CreateConnection();

			if (connection == null)
			{
				throw new DataException(string.Format(Resources.DbProviderFactoryCreateConnectionException, providerName));
			}

			connection.ConnectionString = connectionString;

            if (Log.IsVerboseEnabled)
            {
                _log.Verbose(string.Format(Resources.VerboseDbConnectionCreated, connection.DataSource, connection.Database));
            }

			return connection;
		}
	}
}