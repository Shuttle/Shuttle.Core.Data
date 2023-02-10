using System;
using System.Data.Common;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DatabaseContextServiceExtensions
    {
        public static bool Contains(this IDatabaseContextService databaseContextService, IDatabaseContext context)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNull(context, nameof(context));

            return databaseContextService.Find(databaseContext => databaseContext.Key.Equals(context.Key)) != null;
        }

        public static bool Contains(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextService.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public static bool ContainsConnectionString(this IDatabaseContextService databaseContextService, string connectionString)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            return databaseContextService.FindConnectionString(connectionString) != null;
        }

        public static IDatabaseContext Get(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextService.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception(Resources.DatabaseContextNotFoundException);
        }

        public static IDatabaseContext GetConnectionString(this IDatabaseContextService databaseContextService, string connectionString)
        {
            return databaseContextService.FindConnectionString(connectionString) ?? throw new Exception(Resources.DatabaseContextNotFoundException);
        }

        public static ActiveDatabaseContext Use(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextService.Use(databaseContextService.Get(name));
        }

        public static IDatabaseContext FindConnectionString(this IDatabaseContextService databaseContextService, string connectionString)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            var result = databaseContextService.Find(candidate =>
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

                result = databaseContextService.Find(candidate =>
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