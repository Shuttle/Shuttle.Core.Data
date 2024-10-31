using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class DatabaseContextServiceExtensions
{
    public static void Activate(this IDatabaseContextService databaseContextService, string name)
    {
        Guard.AgainstNull(databaseContextService).Activate(databaseContextService.Get(Guard.AgainstNullOrEmptyString(name)));
    }

    public static bool Contains(this IDatabaseContextService databaseContextService, IDatabaseContext context)
    {
        Guard.AgainstNull(context);

        return Guard.AgainstNull(databaseContextService).Find(databaseContext => databaseContext.Name.Equals(context.Name)) != null;
    }

    public static bool Contains(this IDatabaseContextService databaseContextService, string name)
    {
        Guard.AgainstNullOrEmptyString(name);

        return Guard.AgainstNull(databaseContextService).Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null;
    }

    public static IDatabaseContext Get(this IDatabaseContextService databaseContextService, string name)
    {
        Guard.AgainstNullOrEmptyString(name);

        return Guard.AgainstNull(databaseContextService).Find(databaseContext => databaseContext.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? throw new(string.Format(Resources.DatabaseContextNameNotFoundException, name));
    }

    public static bool IsActive(this IDatabaseContextService databaseContextService, IDatabaseContext context)
    {
        return Guard.AgainstNull(databaseContextService).HasActive && databaseContextService.Active.Name.Equals(Guard.AgainstNull(context).Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsActive(this IDatabaseContextService databaseContextService, string name)
    {
        return Guard.AgainstNull(databaseContextService).HasActive && databaseContextService.Active.Name.Equals(Guard.AgainstNullOrEmptyString(name), StringComparison.InvariantCultureIgnoreCase);
    }
}