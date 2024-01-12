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

        public static IDatabaseContext Get(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextService.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception(Resources.DatabaseContextNotFoundException);
        }

        public static void Activate(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            databaseContextService.Activate(databaseContextService.Get(name));
        }
    }
}