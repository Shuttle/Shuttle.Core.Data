using System;
using System.Data;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class BlockedDbConnection : IDisposable
    {
        public IDbConnection DbConnection { get; }
        private readonly IBlockingSemaphore _blockingSemaphore;

        public BlockedDbConnection(IDbConnection dbConnection, IBlockingSemaphore blockingSemaphore)
        {
            DbConnection = Guard.AgainstNull(dbConnection, nameof(dbConnection));
            _blockingSemaphore = Guard.AgainstNull(blockingSemaphore, nameof(blockingSemaphore));
        }

        public void Dispose()
        {
            _blockingSemaphore.Release();
        }
    }
}