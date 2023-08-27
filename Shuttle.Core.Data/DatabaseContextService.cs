using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private const string AmbientDataKey = "__DatabaseContextService-AmbientData__";

        private class AmbientData
        {
            public readonly List<IDatabaseContext> DatabaseContexts = new List<IDatabaseContext>();
            public IDatabaseContext Current = null;
        }

        public bool HasCurrent => GetAmbientData().Current != null;

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
                if (GetAmbientData().Current == null)
                {
                    throw new InvalidOperationException(Resources.DatabaseContextMissing);
                }

                return GetAmbientData().Current;
            }
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            var current = GetAmbientData().Current;

            GetAmbientData().Current = GetAmbientData().DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));

            if (GetAmbientData().Current == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextKeyNotFoundException, context.Key));
            }

            return new ActiveDatabaseContext(this, current);
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            Guard.AgainstNull(match, nameof(match));

            return GetAmbientData().DatabaseContexts.Find(match);
        }

        public void Add(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            if (Find(context) != null)
            {
                throw new Exception(string.Format(Resources.DuplicateDatabaseContextKeyException, context.Key));
            }
            
            GetAmbientData().DatabaseContexts.Add(context);

            Use(context);
        }

        public void Remove(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            var candidate = Find(context);

            if (candidate == null)
            {
                return;
            }

            if (GetAmbientData().Current != null && candidate.Key.Equals(GetAmbientData().Current.Key))
            {
                GetAmbientData().Current = null;
            }

            GetAmbientData().DatabaseContexts.Remove(candidate);
        }

        private IDatabaseContext Find(IDatabaseContext context)
        {
            return Find(candidate => candidate.Key.Equals(context.Key));
        }

        public ActiveDatabaseContext Use(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            var current = GetAmbientData().Current;

            GetAmbientData().Current = GetAmbientData().DatabaseContexts.Find(candidate =>
                candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (GetAmbientData().Current == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
            }

            return new ActiveDatabaseContext(this, current);
        }
    }
}