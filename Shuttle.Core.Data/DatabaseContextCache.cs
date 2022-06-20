using System;
using System.Data.Common;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextCache : IDatabaseContextCache
    {
        private IDatabaseContext _current;

        public DatabaseContextCache()
        {
            _current = null;
            DatabaseContexts = new DatabaseContextCollection();
        }

        public DatabaseContextCollection DatabaseContexts { get; }

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

        public ActiveDatabaseContext Use(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            var current = _current;

            _current = DatabaseContexts.Find(candidate =>
                candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (_current == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
            }

            return new ActiveDatabaseContext(this, current);
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            var current = _current;

            _current = DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));

            if (_current == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextKeyNotFoundException, context.Key));
            }

            return new ActiveDatabaseContext(this, current);
        }

        public bool Contains(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return DatabaseContexts.Find(candidate =>
                candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public bool ContainsConnectionString(string connectionString)
        {
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            return FindDatabaseContext(connectionString) != null;
        }

        private IDatabaseContext FindDatabaseContext(string connectionString)
        {
            var result = DatabaseContexts.Find(candidate =>
                candidate.Connection.ConnectionString.Equals(connectionString,
                    StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                var matchDbConnectionStringBuilder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString.ToLowerInvariant()
                };

                matchDbConnectionStringBuilder.Remove("password");
                matchDbConnectionStringBuilder.Remove("pwd");

                result = DatabaseContexts.Find(candidate =>
                {
                    var candidateDbConnectionStringBuilder = new DbConnectionStringBuilder
                    {
                        ConnectionString = candidate.Connection.ConnectionString
                    };

                    candidateDbConnectionStringBuilder.Remove("password");
                    candidateDbConnectionStringBuilder.Remove("pwd");

                    return candidateDbConnectionStringBuilder.ConnectionString.Equals(matchDbConnectionStringBuilder.ConnectionString,
                        StringComparison.OrdinalIgnoreCase);
                });
            }

            return result;
        }

        public IDatabaseContext GetConnectionString(string connectionString)
        {
            Guard.AgainstNullOrEmptyString(connectionString, nameof(connectionString));

            var result = FindDatabaseContext(connectionString);

            if (result == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextConnectionStringNotFoundException, connectionString));
            }

            return result;
        }

        public void Add(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            if (Find(context) != null)
            {
                throw new Exception(string.Format(Resources.DuplicateDatabaseContextKeyException, context.Key));
            }

            DatabaseContexts.Add(context);
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

            DatabaseContexts.Remove(candidate);
        }

        public IDatabaseContext Get(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            var result = DatabaseContexts.Find(candidate =>
                candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                throw new Exception(string.Format(Resources.DatabaseContextNameNotFoundException, name));
            }

            return result;
        }

        private IDatabaseContext Find(IDatabaseContext context)
        {
            Guard.AgainstNull(context, nameof(context));

            return DatabaseContexts.Find(candidate => candidate.Key.Equals(context.Key));
        }
    }
}