using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextService
	{
		bool HasCurrent { get; }
        IDatabaseContext Current { get; }
		ActiveDatabaseContext Use(IDatabaseContext context);
		IDatabaseContext Find(Predicate<IDatabaseContext> match);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
	}
}