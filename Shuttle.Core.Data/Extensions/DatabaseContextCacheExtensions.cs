using System;
using System.Data.Common;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DatabaseContextCacheExtensions
    {
        public static bool Contains(this IDatabaseContextCache databaseContextCache, IDatabaseContext context)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNull(context, nameof(context));

            return databaseContextCache.Find(databaseContext => databaseContext.Key.Equals(context.Key)) != null;
        }

        public static bool Contains(this IDatabaseContextCache databaseContextCache, string name)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextCache.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public static bool ContainsConnectionString(this IDatabaseContextCache databaseContextCache, string connectionString)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            return databaseContextCache.FindConnectionString(connectionString) != null;
        }

        public static IDatabaseContext Get(this IDatabaseContextCache databaseContextCache, string name)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextCache.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception(Resources.DatabaseContextNotFoundException);
        }

        public static IDatabaseContext GetConnectionString(this IDatabaseContextCache databaseContextCache, string connectionString)
        {
            return databaseContextCache.FindConnectionString(connectionString) ?? throw new Exception(Resources.DatabaseContextNotFoundException);
        }

        public static ActiveDatabaseContext Use(this IDatabaseContextCache databaseContextCache, string name)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextCache.Use(databaseContextCache.Get(name));
        }

        public static IDatabaseContext FindConnectionString(this IDatabaseContextCache databaseContextCache, string connectionString)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            var result = databaseContextCache.Find(candidate =>
                candidate.Connection.ConnectionString.Equals(connectionString,
                    StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                var matchDbConnectionStringBuilder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString.ToLowerInvariant()
                };

                matchDbConnectionStringBuilder.Remove("password");
                matchDbConnectionStringBuilder.Remove("pwd");

                result = databaseContextCache.Find(candidate =>
                {
                    var candidateDbConnectionStringBuilder = new DbConnectionStringBuilder
                    {
                        ConnectionString = candidate.Connection.ConnectionString
                    };

                    candidateDbConnectionStringBuilder.Remove("password");
                    candidateDbConnectionStringBuilder.Remove("pwd");

                    return candidateDbConnectionStringBuilder.ConnectionString.Equals(matchDbConnectionStringBuilder.ConnectionString,
                        StringComparison.OrdinalIgnoreCase);
                });
            }

            return result;
        }
    }
}