using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DatabaseGateway : IDatabaseGateway
    {
        public async Task<DataTable> GetDataTable(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var results = new DataTable();

            using (var reader = await GetReader(Guard.AgainstNull(databaseContext, nameof(databaseContext)), Guard.AgainstNull(query, nameof(query)), cancellationToken).ConfigureAwait(false))
            {
                results.Load(reader);
            }

            return results;
        }

        public async Task<IEnumerable<DataRow>> GetRows(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var table = await GetDataTable(Guard.AgainstNull(databaseContext, nameof(databaseContext)), Guard.AgainstNull(query, nameof(query)), cancellationToken).ConfigureAwait(false);

            return table.Rows.Cast<DataRow>();
        }

        public async Task<DataRow> GetRow(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            var table = await GetDataTable(Guard.AgainstNull(databaseContext, nameof(databaseContext)), Guard.AgainstNull(query, nameof(query)), cancellationToken).ConfigureAwait(false);

            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            return table.Rows[0];
        }

        public event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated = delegate
        {
        };

        public async Task<IDataReader> GetReader(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            await using (var command = (DbCommand)Guard.AgainstNull(databaseContext, nameof(databaseContext)).CreateCommand(Guard.AgainstNull(query, nameof(query))))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<int> Execute(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            await using (var command = (DbCommand)Guard.AgainstNull(databaseContext, nameof(databaseContext)).CreateCommand(Guard.AgainstNull(query, nameof(query))))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<T> GetScalar<T>(IDatabaseContext databaseContext, IQuery query, CancellationToken cancellationToken = default)
        {
            await using (var command = (DbCommand)Guard.AgainstNull(databaseContext, nameof(databaseContext)).CreateCommand(Guard.AgainstNull(query, nameof(query))))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default;
            }
        }
    }
}