using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextService : IDatabaseContextService
    {
        private readonly List<IDatabaseContext> _databaseContexts = new List<IDatabaseContext>();
        private IDatabaseContext _current = null;

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
}