using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DatabaseContextCache : IDatabaseContextCache
	{
		public IDatabaseContext Current { get; private set; }

		public void Use(string name)
		{
			Current = DatabaseContexts.Find(candidate => candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

			if (Current == null)
			{
				throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
			}
		}

		public void Use(IDatabaseContext context)
		{
			Guard.AgainstNull(context, nameof(context));

			Current = DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));

			if (Current == null)
			{
				throw new Exception(string.Format(Resources.DatabaseContextKeyNotFoundException, context.Key));
			}
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

		public DatabaseContextCollection DatabaseContexts { get; private set; }

		public DatabaseContextCache()
		{
			Current = null;
			DatabaseContexts = new DatabaseContextCollection();
		}
	}
}