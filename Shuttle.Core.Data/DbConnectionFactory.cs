using System.Data;
using System.Data.Common;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
	    private readonly ILog _log;

#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
	    public DbConnectionFactory()
	    {
	        _log = Log.For(this);
	    }
#else
	    private readonly IDbProviderFactories _providerFactories;

	    public DbConnectionFactory(IDbProviderFactories providerFactories)
	    {
            Guard.AgainstNull(providerFactories, nameof(providerFactories));

	        _providerFactories = providerFactories;
	        _log = Log.For(this);
	    }
#endif

        public IDbConnection CreateConnection(string providerName, string connectionString)
		{
#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
		    var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
#else
		    var dbProviderFactory = _providerFactories.GetFactory(providerName);
#endif

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