using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class ActiveDatabaseContext : IDisposable
    {
        private readonly IDatabaseContextService _databaseContextService;
        private readonly IDatabaseContext _current;

        public ActiveDatabaseContext(IDatabaseContextService databaseContextService, IDatabaseContext current)
        {
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
            _current = current;
        }

        public void Dispose()
        {
            if (_current == null)
            {
                return;
            }

            _databaseContextService.Activate(_current);
        }
    }
}