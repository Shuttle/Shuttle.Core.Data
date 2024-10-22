using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class DbConnectionExtensions
{
    private static readonly string[] DefaultSensitiveKeys = { "user id", "uid", "username", "user name", "user", "password", "pwd" };

    public static string SecuredConnectionString(this IDbConnection dbConnection, IEnumerable<string>? sensitiveKeys = null)
    {
        return SecuredConnectionString(Guard.AgainstNull(dbConnection).ConnectionString, sensitiveKeys);
    }

    public static string SecuredConnectionString(string connectionString, IEnumerable<string>? sensitiveKeys = null)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        foreach (var key in (sensitiveKeys ?? DefaultSensitiveKeys).Where(builder.ContainsKey))
        {
            builder[key] = Resources.ConnectionStringHiddenValue;
        }

        return builder.ToString();
    }
}