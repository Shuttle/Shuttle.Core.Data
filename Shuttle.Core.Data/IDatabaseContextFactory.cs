using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;
		event EventHandler<DatabaseContextEventArgs> DatabaseContextReferenced;

        IDatabaseContext Create(string name);
        IDatabaseContext Create();

        IDbConnectionFactory DbConnectionFactory { get; }
        IDbCommandFactory DbCommandFactory { get; }
        IDatabaseContextService DatabaseContextService { get; }
    }
}