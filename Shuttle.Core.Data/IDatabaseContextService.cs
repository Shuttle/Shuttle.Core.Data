using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextService
	{
		event EventHandler<DatabaseContextAsyncLocalValueChangedEventArgs> DatabaseContextAsyncLocalValueChanged;
		event EventHandler<DatabaseContextAsyncLocalValueAssignedEventArgs> DatabaseContextAsyncLocalValueAssigned;

        void Activate(IDatabaseContext databaseContext);
		void Add(IDatabaseContext databaseContext);
		void Remove(IDatabaseContext databaseContext);
		IDatabaseContext Find(Predicate<IDatabaseContext> match);
		IDatabaseContext Current { get; }
		void SetAmbientScope();
	}
}