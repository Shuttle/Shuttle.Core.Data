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

        public DataTable GetDataTable(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var reader = GetReader(query))
            {
                var results = new DataTable();

                if (reader != null)
                {
                    results.Load(reader);
                }

                return results;
            }
        }

        public async Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(query, nameof(query));

            var results = new DataTable();

            using (var reader = await GetReaderAsync(query, cancellationToken).ConfigureAwait(false))
            {
                results.Load(reader);
            }

            return results;
        }

        public IEnumerable<DataRow> GetRows(IQuery query)
        {
            return GetDataTable(query).Rows.Cast<DataRow>();
        }

        public async Task<IEnumerable<DataRow>> GetRowsAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var table = await GetDataTableAsync(query, cancellationToken).ConfigureAwait(false);

            return table.Rows.Cast<DataRow>();
        }

        public DataRow GetRow(IQuery query)
        {
            var table = GetDataTable(query);

            if ((table == null) || (table.Rows.Count == 0))
            {
                return null;
            }

            return table.Rows[0];
        }

        public async Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            var table = await GetDataTableAsync(query, cancellationToken).ConfigureAwait(false);

            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            return table.Rows[0];
        }

        public event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated = delegate
        {
        };

        public IDataReader GetReader(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Current.CreateCommand(query))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return command.ExecuteReader();
            }
        }

        public async Task<IDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(query, nameof(query));

            var command = (DbCommand)await (_databaseContextService.Current.CreateCommandAsync(query).ConfigureAwait(false));
            
            await using var _ = command.ConfigureAwait(false);
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public int Execute(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Current.CreateCommand(query))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return command.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(query, nameof(query));

            await using (var command = (DbCommand)await (_databaseContextService.Current.CreateCommandAsync(query).ConfigureAwait(false)))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public T GetScalar<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Current.CreateCommand(query))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                var scalar = command.ExecuteScalar();

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default(T);
            }
        }

        public async Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(query, nameof(query));

            var command = (DbCommand)await _databaseContextService.Current.CreateCommandAsync(query).ConfigureAwait(false);

            await using (command.ConfigureAwait(false))
            {
                DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

                var scalar = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default;
            }
        }
    }
}