using System.Data;
using System.Data.Common;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DbConnectionExtensions
    {
        static readonly string[] Keys = { "user id", "uid", "username", "user name", "user", "password", "pwd" };

        public static string SecuredConnectionString(this IDbConnection dbConnection)
        {
            Guard.AgainstNull(dbConnection, "dbConnection");

            return SecuredConnectionString(dbConnection.ConnectionString);
        }

        public static string SecuredConnectionString(string connectionString)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            foreach (var key in Keys.Where(builder.ContainsKey))
            {
                builder[key] = Resources.ConnectionStringHiddenValue;
            }

            return builder.ToString();
        }
    }
}