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

            return databaseContextService.Find(databaseContext => databaseContext.Name.Equals(context.Name)) != null;
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

            return databaseContextService.Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
        }

        public static void Activate(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            databaseContextService.Activate(databaseContextService.Get(name));
        }

        public static bool IsActive(this IDatabaseContextService databaseContextService, IDatabaseContext context)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNull(context, nameof(context));

            return databaseContextService.HasActive && databaseContextService.Active.Name.Equals(context.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsActive(this IDatabaseContextService databaseContextService, string name)
        {
            Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return databaseContextService.HasActive && databaseContextService.Active.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}