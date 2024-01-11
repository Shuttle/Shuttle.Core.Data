using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Core.Data
{
    public class BlockedDbCommand : IDisposable
    {
        private readonly SemaphoreSlim _dbDataReaderLock;
        private readonly DbCommand _dbCommand;
        private readonly IBlockingSemaphore _blockingSemaphore;

        public BlockedDbCommand(DbCommand dbCommand, IBlockingSemaphore blockingSemaphore, SemaphoreSlim dbDataReaderLock)
        {
            _dbCommand = Guard.AgainstNull(dbCommand, nameof(dbCommand));
            _blockingSemaphore = Guard.AgainstNull(blockingSemaphore, nameof(blockingSemaphore));
            _dbDataReaderLock = Guard.AgainstNull(dbDataReaderLock, nameof(dbDataReaderLock));
        }

        public void Dispose()
        {
            _dbCommand.Dispose();
            _blockingSemaphore.Release();
        }

        public BlockedDbDataReader ExecuteReader()
        {
            _dbDataReaderLock.Wait(CancellationToken.None);

            return new BlockedDbDataReader(_dbCommand.ExecuteReader(), new BlockingSemaphoreSlim(_dbDataReaderLock));
        }

        public async Task<BlockedDbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            await _dbDataReaderLock.WaitAsync(cancellationToken);

            return new BlockedDbDataReader(await _dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false), new BlockingSemaphoreSlim(_dbDataReaderLock));
        }

        public int ExecuteNonQuery()
        {
            return _dbCommand.ExecuteNonQuery();
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return await _dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public object ExecuteScalar()
        {
            return _dbCommand.ExecuteScalar();
        }

        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return await _dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}