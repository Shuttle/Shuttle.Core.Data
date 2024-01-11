using System;
using System.Data;
using System.Data.Common;

namespace Shuttle.Core.Data
{
	public interface IDatabaseContextService
	{
		ActiveDatabaseContext Activate(IDatabaseContext context);
		void Add(IDatabaseContext context);
		void Remove(IDatabaseContext context);
		IDatabaseContext Find(Predicate<IDatabaseContext> match);
		IDatabaseContext Current { get; }
	}
}