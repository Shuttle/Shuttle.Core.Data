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

            return databaseGateway.Execute(new RawQuery(sql), cancellationToken);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.ExecuteAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static int Execute(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.Execute(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.ExecuteAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static DataTable GetDataTable(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetDataTable(new RawQuery(sql), cancellationToken);
        }

        public static async Task<DataTable> GetDataTableAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTableAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static DataTable GetDataTable(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetDataTable(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<DataTable> GetDataTableAsync(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetDataTableAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }
        
        public static IDataReader GetReader(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetReader(new RawQuery(sql), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReaderAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }
        
        public static async Task<IDataReader> GetReader(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }
        
        public static async Task<IDataReader> GetReaderAsync(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetReaderAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static DataRow GetRow(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRow(new RawQuery(sql), cancellationToken);
        }

        public static async Task<DataRow> GetRowAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static DataRow GetRow(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRow(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<DataRow> GetRowAsync(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<DataRow> GetRows(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRows(new RawQuery(sql), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRowsAsync(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowsAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<DataRow> GetRows(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetRows(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<IEnumerable<DataRow>> GetRowsAsync(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetRowsAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static T GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetScalar<T>(new RawQuery(sql), cancellationToken);
        }

        public static async Task<T> GetScalarAsync<T>(this IDatabaseGateway databaseGateway, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalarAsync<T>(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T GetScalar<T>(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return databaseGateway.GetScalar<T>(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<T> GetScalarAsync<T>(this IDatabaseGateway databaseGateway, string sql, object parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            return await databaseGateway.GetScalarAsync<T>(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }
    }
}