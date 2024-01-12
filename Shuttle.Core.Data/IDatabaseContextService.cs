using System;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextService
	{
		void Activate(IDatabaseContext context);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
		IDatabaseContext Find(Predicate<IDatabaseContext> match);
		IDatabaseContext Current { get; }
	}
}