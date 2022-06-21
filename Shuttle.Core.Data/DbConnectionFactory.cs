using System.Data;
using Microsoft.Extensions.Logging;
#if !NETSTANDARD2_0
using System.Data.Common;
#endif
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
		private readonly ILogger<DbConnectionFactory> _logger;

#if (NETSTANDARD2_0)
		private readonly IDbProviderFactories _providerFactories;

	    public DbConnectionFactory(ILogger<DbConnectionFactory> logger, IDbProviderFactories providerFactories)
	    {
            Guard.AgainstNull(logger, nameof(logger));
            Guard.AgainstNull(providerFactories, nameof(providerFactories));

            _logger = logger;
            _providerFactories = providerFactories;
	    }
#else
		public DbConnectionFactory(ILogger<DbConnectionFactory> logger)
		{
			Guard.AgainstNull(logger, nameof(logger));
			
			_logger = logger;
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

            if (_logger.IsEnabled(LogLevel.Trace))
            {
	            _logger.LogTrace(string.Format(Resources.VerboseDbConnectionCreated, connection.DataSource, connection.Database));
            }

			return connection;
		}
	}
}