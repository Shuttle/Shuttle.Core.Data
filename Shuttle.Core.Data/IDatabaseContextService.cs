using System;

namespace Shuttle.Core.Data;

public interface IDatabaseContextService
{
    bool Activate(IDatabaseContext databaseContext);
    IDisposable Add(IDatabaseContext databaseContext);
    void Remove(IDatabaseContext databaseContext);
    IDatabaseContext? Find(Predicate<IDatabaseContext> match);
    IDatabaseContext Active { get; }
    bool HasActive { get; }
}