using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;

        IDatabaseContext Create(string connectionStringName, TimeSpan? timeout = null);
        IDatabaseContext Create(TimeSpan? timeout = null);
    }
}