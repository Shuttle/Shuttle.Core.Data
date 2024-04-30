using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseContextEventArgs : EventArgs
    {
        public DatabaseContextEventArgs(IDatabaseContext databaseContext)
        {
            DatabaseContext = Guard.AgainstNull(databaseContext, nameof(databaseContext));
        }

        public IDatabaseContext DatabaseContext { get; }
    }
}