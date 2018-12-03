using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DatabaseContextCache : IDatabaseContextCache
	{
		public IDatabaseContext Current { get; private set; }

		public ActiveDatabaseContext Use(string name)
		{
		    var current = Current;

            Current = DatabaseContexts.Find(candidate => candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

			if (Current == null)
			{
				throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
			}

            return new ActiveDatabaseContext(this, current);
		}

		public ActiveDatabaseContext Use(IDatabaseContext context)
		{
			Guard.AgainstNull(context, nameof(context));

		    var current = Current;

			Current = DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));

			if (Current == null)
			{
				throw new Exception(string.Format(Resources.DatabaseContextKeyNotFoundException, context.Key));
			}

		    return new ActiveDatabaseContext(this, current);
        }

		public bool Contains(string connectionString)
		{
			return DatabaseContexts.Find(candidate => candidate.Connection.ConnectionString.Equals(connectionString, StringComparison.OrdinalIgnoreCase)) != null;
		}

		public void Add(IDatabaseContext context)
		{
			if (Find(context) != null)
			{
				throw new Exception(string.Format(Resources.DuplicateDatabaseContextKeyException, context.Key));
			}
			
			DatabaseContexts.Add(context);
		    Use(context);
		}

        private IDatabaseContext Find(IDatabaseContext context)
		{
			return DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));
		}

		public void Remove(IDatabaseContext context)
		{
			var candidate = Find(context);

			if (candidate == null)
			{
				return;
			}

			if (Current != null && candidate.Key.Equals(Current.Key))
			{
				Current = null;
			}

			DatabaseContexts.Remove(candidate);
		}

		public IDatabaseContext Get(string connectionString)
		{
			var result = DatabaseContexts.Find(candidate => candidate.Connection.ConnectionString.Equals(connectionString, StringComparison.OrdinalIgnoreCase));

			if (result == null)
			{
				throw new Exception(string.Format(Resources.DatabaseContextConnectionStringNotFoundException, DbConnectionExtensions.SecuredConnectionString(connectionString)));
			}

			return result;
		}

		public DatabaseContextCollection DatabaseContexts { get; }

		public DatabaseContextCache()
		{
			Current = null;
			DatabaseContexts = new DatabaseContextCollection();
		}
	}
}