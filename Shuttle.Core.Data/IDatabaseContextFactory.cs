using System;

namespace Shuttle.Core.Data;

public interface IDatabaseContextFactory
{
    IDatabaseContext Create(string connectionStringName);
    IDatabaseContext Create();
    event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;
}