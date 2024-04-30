using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextFactory
	{
		event EventHandler<DatabaseContextEventArgs> DatabaseContextCreated;

        IDatabaseContext Create(string connectionStringName);
        IDatabaseContext Create();
    }
}