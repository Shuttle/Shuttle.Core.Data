using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DatabaseGatewayExtensions
    {
        public static int Execute(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.Execute(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.ExecuteAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static int Execute(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.Execute(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.ExecuteAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static DataTable GetDataTable(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetDataTable(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<DataTable> GetDataTableAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTableAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static DataTable GetDataTable(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetDataTable(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<DataTable> GetDataTableAsync(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTableAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }
        
        public static IDataReader GetReader(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetReader(RawQuery.Create(sql), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReaderAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReaderAsync(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static DataRow GetRow(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRow(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<DataRow> GetRowAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static DataRow GetRow(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRow(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<DataRow> GetRowAsync(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<DataRow> GetRows(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRows(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRowsAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowsAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<DataRow> GetRows(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRowsAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRowsAsync(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowsAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static T GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetScalar<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> GetScalarAsync<T>(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalarAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetScalar<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> GetScalarAsync<T>(this IDatabaseGateway databaseGateway, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalarAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }
    }
}