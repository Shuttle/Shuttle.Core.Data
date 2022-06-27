using System;
using System.Data;
#if !NETSTANDARD2_0
using System.Data.Common;
#endif
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
#if (NETSTANDARD2_0)
		private readonly IDbProviderFactories _providerFactories;

	    public DbConnectionFactory(IDbProviderFactories providerFactories)
	    {
            Guard.AgainstNull(providerFactories, nameof(providerFactories));

            _providerFactories = providerFactories;
	    }
#endif

		public event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated = delegate
		{
		};

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

			DbConnectionCreated.Invoke(this, new DbConnectionCreatedEventArgs(connection));

			return connection;
		}
	}
}