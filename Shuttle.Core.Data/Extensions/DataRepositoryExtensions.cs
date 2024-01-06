using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DataRepositoryExtensions
    {
        public static bool Contains<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.Contains(new RawQuery(sql), cancellationToken);
        }

        public static async Task<bool> ContainsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static bool Contains<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.Contains(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<bool> ContainsAsync<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static T FetchItem<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItem(new RawQuery(sql), cancellationToken);
        }

        public static async Task<T> FetchItemAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T FetchItem<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItem(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<T> FetchItemAsync<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItems(new RawQuery(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItemsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItems(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItemsAsync<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRow(new RawQuery(sql), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRowAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRow(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRowAsync<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<MappedRow<T>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRows(new RawQuery(sql), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(new RawQuery(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<MappedRow<T>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRows(new RawQuery(sql).AddParameters(parameters), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync<T>(this IDataRepository<T> dataRepository, string sql, object parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(new RawQuery(sql).AddParameters(parameters), cancellationToken).ConfigureAwait(false);
        }
    }
}