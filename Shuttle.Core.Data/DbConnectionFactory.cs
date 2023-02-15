using System;
using System.Data;
using System.Data.Common;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
		public event EventHandler<DbConnectionCreatedEventArgs> DbConnectionCreated = delegate
		{
		};

		public IDbConnection Create(string providerName, string connectionString)
		{
			Guard.AgainstNullOrEmptyString(providerName, nameof(providerName));
			Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

			var dbProviderFactory = DbProviderFactories.GetFactory(providerName);
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