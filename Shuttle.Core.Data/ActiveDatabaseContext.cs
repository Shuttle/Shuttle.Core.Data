using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ActiveDatabaseContext : IDisposable
    {
        private readonly IDatabaseContextCache _databaseContextCache;
        private readonly IDatabaseContext _current;

        public ActiveDatabaseContext(IDatabaseContextCache databaseContextCache, IDatabaseContext current)
        {
            Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

            _databaseContextCache = databaseContextCache;
            _current = current;
        }

        public void Dispose()
        {
            if (_current == null)
            {
                return;
            }

            _databaseContextCache.Use(_current);
        }
    }
}