using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DataRepositoryExtensions
    {
        public static async Task<bool> Contains<T>(this IDataRepository<T> dataRepository, string sql) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<bool> Contains<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<bool> Contains<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<bool> Contains<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.ContainsAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> FetchItem<T>(this IDataRepository<T> dataRepository, string sql) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<T> FetchItem<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<T> FetchItem<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> FetchItem<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItems<T>(this IDataRepository<T> dataRepository, string sql) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchItemsAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<MappedRow<T>> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<MappedRow<T>> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<MappedRow<T>> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters, CancellationToken cancellationToken) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return await dataRepository.FetchMappedRowsAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }
    }
}