using Shuttle.Core.Contract;
using System;
using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private class CallContextService : IDatabaseContextService
        {
            private readonly List<IDatabaseContext> _databaseContexts = new List<IDatabaseContext>();
            private IDatabaseContext _current;

            public CallContextService()
            {
                _current = null;
            }

            public bool HasCurrent => _current != null;

            public IDatabaseContext Current
            {
                get
                {
                    if (_current == null)
                    {
                        throw new InvalidOperationException(Resources.DatabaseContextMissing);
                    }

                    return _current;
                }
            }

            public ActiveDatabaseContext Use(IDatabaseContext context)
            {
                Guard.AgainstNull(context, nameof(context));

                var current = _current;

                _current = _databaseContexts.Find(candidate => candidate.Key.Equals(context.Key));

                if (_current == null)
                {
                    throw new Exception(string.Format(Resources.DatabaseContextKeyNotFoundException, context.Key));
                }

                return new ActiveDatabaseContext(this, current);
            }

            public IDatabaseContext Find(Predicate<IDatabaseContext> match)
            {
                Guard.AgainstNull(match, nameof(match));

                return _databaseContexts.Find(match);
            }

            public void Add(IDatabaseContext context)
            {
                Guard.AgainstNull(context, nameof(context));

                if (Find(context) != null)
                {
                    throw new Exception(string.Format(Resources.DuplicateDatabaseContextKeyException, context.Key));
                }

                _databaseContexts.Add(context);

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

                if (_current != null && candidate.Key.Equals(_current.Key))
                {
                    _current = null;
                }

                _databaseContexts.Remove(candidate);
            }

            private IDatabaseContext Find(IDatabaseContext context)
            {
                return Find(candidate => candidate.Key.Equals(context.Key));
            }

            public ActiveDatabaseContext Use(string name)
            {
                Guard.AgainstNullOrEmptyString(name, nameof(name));

                var current = _current;

                _current = _databaseContexts.Find(candidate =>
                    candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (_current == null)
                {
                    throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
                }

                return new ActiveDatabaseContext(this, current);
            }
        }
        
        public IDatabaseContext Current => Guarded().Current;

        public ActiveDatabaseContext Use(string name)
        {
            return Guarded().Use(name);
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            return Guarded().Use(context);
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            return Guarded().Find(match);
        }

        public bool Contains(string connectionString)
        {
            return Guarded().Contains(connectionString);
        }

        public bool ContainsConnectionString(string connectionString)
        {
            return Guarded().ContainsConnectionString(connectionString);
        }

        public IDatabaseContext GetConnectionString(string connectionString)
        {
            return Guarded().GetConnectionString(connectionString);
        }

        public void Add(IDatabaseContext context)
        {
            Guarded().Add(context);
            Guarded().Use(context);
        }

        public void Remove(IDatabaseContext context)
        {
            Guarded().Remove(context);
        }

        public bool HasCurrent => Guarded().HasCurrent;

        public IDatabaseContext Get(string connectionString)
        {
            return Guarded().Get(connectionString);
        }

        private IDatabaseContextService Guarded()
        {
            const string key = "__database-context-service-item__";

            var result = (IDatabaseContextService)Threading.CallContext.GetData(key);

            if (result != null)
            {
                return result;
            }

            Threading.CallContext.SetData(key, new CallContextService());

            return (IDatabaseContextService)Threading.CallContext.GetData(key);
        }
    }
}