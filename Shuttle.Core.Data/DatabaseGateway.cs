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
        private readonly IDatabaseContextService _databaseContextService;

        public DatabaseGateway(IDatabaseContextService databaseContextService)
        {
            _databaseContextService = Guard.AgainstNull(databaseContextService, nameof(databaseContextService));
        }

        public async Task<DataTable> GetDataTable(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            using var reader = GetReader(query, cancellationToken);

            var results = new DataTable();

            if (reader != null)
            {
                results.Load(await reader);
            }

            return results;
        }

        public Task<IEnumerable<DataRow>> GetRows(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(GetDataTable(query, cancellationToken).Result.Rows.Cast<DataRow>());
        }

        public async Task<DataRow> GetRow(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var table = await GetDataTable(query, cancellationToken);

            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            return table.Rows[0];
        }

        public event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated = delegate
        {
        };

        public async Task<DbDataReader> GetReader(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            await using var command = (DbCommand)_databaseContextService.Current.CreateCommand(query);

            DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

            return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> Execute(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            await using var command = (DbCommand)_databaseContextService.Current.CreateCommand(query);

            DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<T> GetScalar<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            var command = (DbCommand)_databaseContextService.Current.CreateCommand(query);

            await using (command.ConfigureAwait(false))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default;
            }
        }
    }
}