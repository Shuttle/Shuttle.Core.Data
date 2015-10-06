using System.Data;
using System.Data.Common;
using System.Linq;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public static class DbConnectionExtensions
	{
		static readonly string[] keys = { "user id", "uid", "username", "user name", "user", "password", "pwd" };

		public static string SecuredConnectionString(this IDbConnection dbConnection)
		{
			Guard.AgainstNull(dbConnection, "dbConnection");

			return SecuredConnectionString(dbConnection.ConnectionString);
		}

		public static string SecuredConnectionString(string connectionString)
		{
			var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

			foreach (var key in keys.Where(builder.ContainsKey))
			{
				builder[key] = DataResources.ConnectionStringHiddenValue;
			}

			return builder.ToString();
		}
	}
}