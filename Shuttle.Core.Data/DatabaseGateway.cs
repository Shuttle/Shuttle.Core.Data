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

        public DataTable GetDataTable(IQuery query, CancellationToken cancellationToken = default)
        {
            return GetDataTableAsync(query, cancellationToken, true).GetAwaiter().GetResult();
        }

        public async Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            return await GetDataTableAsync(query, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task<DataTable> GetDataTableAsync(IQuery query, CancellationToken cancellationToken, bool sync)
        {
            Guard.AgainstNull(query, nameof(query));

            var results = new DataTable();

            using (var reader = sync ? GetReader(query, cancellationToken) : await GetReaderAsync(query, cancellationToken).ConfigureAwait(false))
            {
                results.Load(reader);
            }

            return results;
        }

        public IEnumerable<DataRow> GetRows(IQuery query, CancellationToken cancellationToken = default)
        {
            return GetDataTable(query, cancellationToken).Rows.Cast<DataRow>();
        }

        public async Task<IEnumerable<DataRow>> GetRowsAsync(IQuery query, CancellationToken cancellationToken = default)
        {

            return (await GetDataTableAsync(query, cancellationToken).ConfigureAwait(false)).Rows.Cast<DataRow>();
        }

        public DataRow GetRow(IQuery query, CancellationToken cancellationToken = default)
        {
            return GetRowAsync(query, cancellationToken, true).GetAwaiter().GetResult();
        }

        public async Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            return await GetRowAsync(query, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task<DataRow> GetRowAsync(IQuery query, CancellationToken cancellationToken, bool sync)
        {
            var table = sync ? GetDataTable(query, cancellationToken) : await GetDataTableAsync(query, cancellationToken).ConfigureAwait(false);

            if (table == null || table.Rows.Count == 0)
            {
                return null;
            }

            return table.Rows[0];
        }

        public DbDataReader GetReader(IQuery query, CancellationToken cancellationToken = default)
        {
            return GetReaderAsync(query, cancellationToken, true).GetAwaiter().GetResult();
        }

        public async Task<DbDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            return await GetReaderAsync(query, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task<DbDataReader> GetReaderAsync(IQuery query, CancellationToken cancellationToken, bool sync)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Active.CreateCommand(query))
            {
                return sync ? command.ExecuteReader() : await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public int Execute(IQuery query, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(query, cancellationToken, true).GetAwaiter().GetResult();
        }

        public async Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(query, cancellationToken, false).ConfigureAwait(false);
        }

        private async Task<int> ExecuteAsync(IQuery query, CancellationToken cancellationToken, bool sync)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Active.CreateCommand(query))
            {
                return sync ? command.ExecuteNonQuery() : await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public T GetScalar<T>(IQuery query, CancellationToken cancellationToken = default)
        {
            return GetScalarAsync<T>(query, cancellationToken, true).GetAwaiter().GetResult();
        }

        public async Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken = default)
        {
            return await GetScalarAsync<T>(query, cancellationToken, false).ConfigureAwait(false);
        }

        public async Task<T> GetScalarAsync<T>(IQuery query, CancellationToken cancellationToken, bool sync)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var command = _databaseContextService.Active.CreateCommand(query))
            {
                var scalar = sync ? command.ExecuteScalar() : await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default;
            }
        }
    }
}