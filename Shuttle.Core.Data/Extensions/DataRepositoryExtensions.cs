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

            return dataRepository.Contains(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<bool> ContainsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static bool Contains<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.Contains(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<bool> ContainsAsync<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static T FetchItem<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItem(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> FetchItemAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T FetchItem<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.Contains(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> FetchItemAsync<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItems(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItemsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItems(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItemsAsync<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRow(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRowAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRow(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRowAsync<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<MappedRow<T>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRows(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<MappedRow<T>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRows(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRowsAsync<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }
    }
}