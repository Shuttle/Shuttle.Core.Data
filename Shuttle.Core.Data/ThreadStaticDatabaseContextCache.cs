using System;

namespace Shuttle.Core.Data
{
    public class ThreadStaticDatabaseContextService : IDatabaseContextService
	{
		[ThreadStatic] private static DatabaseContextService _service;

		public bool HasCurrent => GuardedCache().HasCurrent;

        public IDatabaseContext Current => GuardedCache().Current;

	    public ActiveDatabaseContext Use(string name)
		{
			return GuardedCache().Use(name);
		}

		public ActiveDatabaseContext Use(IDatabaseContext context)
		{
			return GuardedCache().Use(context);
		}

		public IDatabaseContext Find(Predicate<IDatabaseContext> match)
		{
			return GuardedCache().Find(match);
		}

        public void Add(IDatabaseContext context)
		{
			GuardedCache().Add(context);
		}

		public void Remove(IDatabaseContext context)
		{
			GuardedCache().Remove(context);
		}

		public IDatabaseContext Get(string name)
		{
			return GuardedCache().Get(name);
		}

		private static DatabaseContextService GuardedCache()
		{
			return _service ?? (_service = new DatabaseContextService());
		}
	}
}