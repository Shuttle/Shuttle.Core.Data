using System;
using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public class DatabaseContextCollection
    {
        public IEnumerable<IDatabaseContext> DatabaseContexts => _databaseContexts.AsReadOnly();

        public IDatabaseContext ActiveDatabaseContext { get; private set; }

        private readonly List<IDatabaseContext> _databaseContexts = new List<IDatabaseContext>();

        internal void Activate(IDatabaseContext databaseContext)
        {
            ActiveDatabaseContext = databaseContext;
        }

        internal void Add(IDatabaseContext context)
        {
            if (_databaseContexts.Find(candidate => candidate.Name.Equals(context.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                throw new Exception(string.Format(Resources.DuplicateDatabaseContextException, context.Name));
            }

            _databaseContexts.Add(context);
        }

        internal void Remove(IDatabaseContext databaseContext)
        {
            if (_databaseContexts.Find(candidate => candidate.Name.Equals(databaseContext.Name)) == null)
            {
                throw new InvalidOperationException(string.Format(Resources.DatabaseContextKeyNotFoundException, databaseContext.Name, databaseContext.Name));
            }

            if (ActiveDatabaseContext != null && databaseContext.Name.Equals(ActiveDatabaseContext.Name))
            {
                ActiveDatabaseContext = null;
            }

            _databaseContexts.Remove(databaseContext);
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            return _databaseContexts.Find(match);
        }
    }
}