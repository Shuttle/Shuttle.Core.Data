using System;
using System.Data.Common;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class BlockedDbDataReader : IDisposable
    {
        private readonly IBlockingSemaphore _blockingSemaphore;
        public DbDataReader DbDataReader { get; }

        public BlockedDbDataReader(DbDataReader dbDataReader, IBlockingSemaphore blockingSemaphore)
        {
            DbDataReader = Guard.AgainstNull(dbDataReader, nameof(dbDataReader));
            _blockingSemaphore = Guard.AgainstNull(blockingSemaphore, nameof(blockingSemaphore));
        }

        public void Dispose()
        {
            DbDataReader.Dispose();
            _blockingSemaphore.Release();
        }
    }
}