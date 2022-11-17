using System;

namespace Shuttle.Core.Data
{
	public class ThreadStaticDatabaseContextCache : IDatabaseContextCache
	{
		[ThreadStatic] private static DatabaseContextCache _cache;

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

		public bool Contains(string name)
		{
			return GuardedCache().Contains(name);
		}

        public bool ContainsConnectionString(string connectionString)
        {
            return GuardedCache().ContainsConnectionString(connectionString);
        }

        public IDatabaseContext GetConnectionString(string connectionString)
        {
            return GuardedCache().GetConnectionString(connectionString);
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

		private static DatabaseContextCache GuardedCache()
		{
			return _cache ?? (_cache = new DatabaseContextCache());
		}
	}
}