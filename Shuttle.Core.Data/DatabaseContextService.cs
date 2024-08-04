using System;
using System.Linq;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private readonly DatabaseContextCollection _databaseContextCollection = new DatabaseContextCollection();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public IDatabaseContext Active
        {
            get
            {
                _lock.Wait();

                try
                {
                    return GetDatabaseContextCollection().ActiveDatabaseContext ?? throw new InvalidOperationException(Resources.DatabaseContextMissing);
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public bool HasActive
        {
            get
            {
                _lock.Wait();

                try
                {
                    return GetDatabaseContextCollection().ActiveDatabaseContext != null;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public void Activate(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                var current = GetDatabaseContextCollection().ActiveDatabaseContext;

                if (current != null && current.Name.Equals(databaseContext.Name))
                {
                    throw new Exception(string.Format(Resources.DatabaseContextAlreadyActiveException, databaseContext.Name));
                }

                var activate = GetDatabaseContextCollection().DatabaseContexts.FirstOrDefault(item => item.Name.Equals(databaseContext.Name, StringComparison.InvariantCultureIgnoreCase));

                if (activate == null)
                {
                    throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, databaseContext.Name));
                }

                GetDatabaseContextCollection().Activate(activate);
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Add(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                GetDatabaseContextCollection().Add(databaseContext);
            }
            finally
            {
                _lock.Release();
            }

            Activate(databaseContext);
        }

        public void Remove(IDatabaseContext databaseContext)
        {
            Guard.AgainstNull(databaseContext, nameof(databaseContext));

            _lock.Wait();

            try
            {
                GetDatabaseContextCollection().Remove(databaseContext);
            }
            finally
            {
                _lock.Release();
            }
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            Guard.AgainstNull(match, nameof(match));

            _lock.Wait();

            try
            {
                return GetDatabaseContextCollection().Find(match);
            }
            finally
            {
                _lock.Release();
            }
        }

        private DatabaseContextCollection GetDatabaseContextCollection()
        {
            return DatabaseContextScope.Current ?? _databaseContextCollection;
        }
    }
}