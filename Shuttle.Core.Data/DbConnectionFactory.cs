using System.Data;
#if !NETSTANDARD2_0
using System.Data.Common;
#endif
using Shuttle.Core.Logging;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
	    private readonly ILog _log;

#if (NETSTANDARD2_0)
		private readonly IDbProviderFactories _providerFactories;

	    public DbConnectionFactory(IDbProviderFactories providerFactories)
	    {
            Guard.AgainstNull(providerFactories, nameof(providerFactories));

	        _providerFactories = providerFactories;
	        _log = Log.For(this);
	    }
#else
		public DbConnectionFactory()
		{
			_log = Log.For(this);
		}
#endif

		public IDbConnection CreateConnection(string providerName, string connectionString)
		{
			Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
			Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

#if NETSTANDARD2_0
			var dbProviderFactory = _providerFactories.GetFactory(providerName);
#else
			var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
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