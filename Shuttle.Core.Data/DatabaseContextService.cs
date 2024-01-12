using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private const string AmbientDataKey = "__DatabaseContextService-AmbientData__";

        private class AmbientData
        {
            public readonly List<IDatabaseContext> DatabaseContexts = new List<IDatabaseContext>();
            public IDatabaseContext ActiveDatabaseContext = null;
        }

        private AmbientData GetAmbientData()
        {
            var result = AmbientContext.GetData(AmbientDataKey) as AmbientData;

            if (result == null)
            {
                result = new AmbientData();

                AmbientContext.SetData(AmbientDataKey, result);
            }

            return result;
        }

        public IDatabaseContext Current
        {
            get
            {
                _lock.Wait();

                try
                {
                    if (GetAmbientData().ActiveDatabaseContext == null)
                    {
                        throw new InvalidOperationException(Resources.DatabaseContextMissing);
                    }

                    return GetAmbientData().ActiveDatabaseContext;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public void Activate(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            _lock.Wait();

            try
            {
                var current = GetAmbientData().ActiveDatabaseContext;

                if (current != null && current.Name.Equals(context.Name))
                {
                    throw new Exception(string.Format(Resources.DatabaseContextAlreadyActiveException, context.Name));
                }

                var active = GetAmbientData().DatabaseContexts.FirstOrDefault(item => item.Name.Equals(context.Name, StringComparison.InvariantCultureIgnoreCase));

                if (active == null)
                {
                    throw new Exception(string.Format(Resources.DatabaseContextNotFoundException, context.Name));
                }

                GetAmbientData().ActiveDatabaseContext = active;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Add(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            _lock.Wait();

            try
            {
                if (Find(context) != null)
                {
                    throw new Exception(string.Format(Resources.DuplicateDatabaseContextKeyException, context.Key));
                }
            
                GetAmbientData().DatabaseContexts.Add(context);
            }
            finally
            {
                _lock.Release();
            }

            Activate(context);
        }

        public void Remove(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            _lock.Wait();

            try
            {
                var candidate = Find(context);

                if (candidate == null)
                {
                    return;
                }

                if (GetAmbientData().ActiveDatabaseContext != null && candidate.Key.Equals(GetAmbientData().ActiveDatabaseContext.Key))
                {
                    GetAmbientData().ActiveDatabaseContext = null;
                }

                GetAmbientData().DatabaseContexts.Remove(candidate);
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
                return GetAmbientData().DatabaseContexts.Find(match);
            }
            finally
            {
                _lock.Release();
            }
        }

        private IDatabaseContext Find(Predicate<IDatabaseContext> match, bool locked)
        {
            Guard.AgainstNull(match, nameof(match));

            if (locked)
            {
                _lock.Wait();
            }

            try
            {
                return GetAmbientData().DatabaseContexts.Find(match);
            }
            finally
            {
                if (locked)
                {
                    _lock.Release();
                }
            }
        }

        private IDatabaseContext Find(IDatabaseContext context)
        {
            return Find(candidate => candidate.Key.Equals(context.Key), false);
        }
    }
}