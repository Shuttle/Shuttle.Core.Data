using System;

namespace Shuttle.Core.Data
{
	public class ThreadStaticDatabaseContextCache : IDatabaseContextCache
	{
		[ThreadStatic] private static DatabaseContextCache _cache;

		public IDatabaseContext Current => GuardedCache().Current;

	    public ActiveDatabaseContext Use(string name)
		{
			return GuardedCache().Use(name);
		}

		public ActiveDatabaseContext Use(IDatabaseContext context)
		{
			return GuardedCache().Use(context);
		}

		public bool Contains(string connectionString)
		{
			return GuardedCache().Contains(connectionString);
		}

		public void Add(IDatabaseContext context)
		{
			GuardedCache().Add(context);
		}

		public void Remove(IDatabaseContext context)
		{
			GuardedCache().Remove(context);
		}

		public IDatabaseContext Get(string connectionString)
		{
			return GuardedCache().Get(connectionString);
		}

		private static DatabaseContextCache GuardedCache()
		{
			return _cache ?? (_cache = new DatabaseContextCache());
		}
	}
}