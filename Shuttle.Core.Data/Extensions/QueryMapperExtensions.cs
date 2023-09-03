using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class QueryMapperExtensions
    {
        public static dynamic MapItem(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapItem(RawQuery.Create(sql), cancellationToken);
        }
        
        public static async Task<dynamic> MapItemAsync(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItemAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }
        
        public static async Task<dynamic> MapItem(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItemAsync(RawQuery.Create(sql, parameters), cancellationToken);
        }
        
        public static async Task<dynamic> MapItemAsync(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItemAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<dynamic> MapItems(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapItems(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<dynamic>> MapItemsAsync(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItemsAsync(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<dynamic> MapItems(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapItems(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<dynamic>> MapItemsAsync(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItemsAsync(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static T MapObject<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObject<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> MapObjectAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjectAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T MapObject<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObject<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> MapObjectAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjectAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> MapObjects<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObjects<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapObjectsAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjectsAsync<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static IEnumerable<T> MapObjects<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObjects<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapObjectsAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjectsAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> MapRow<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRow<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<MappedRow<T>> MapRowAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRowAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static MappedRow<T> MapRow<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRow<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<MappedRow<T>> MapRowAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRowAsync<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static IEnumerable<MappedRow<T>> MapRows<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRows<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRowsAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<MappedRow<T>> MapRows<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRows<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRowsAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static T MapValue<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValue<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> MapValueAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValueAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static T MapValue<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValue<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> MapValueAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValueAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> MapValues<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValues<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapValuesAsync<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValuesAsync<T>(RawQuery.Create(sql), cancellationToken).ConfigureAwait(false);
        }

        public static IEnumerable<T> MapValues<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValues<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapValuesAsync<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValuesAsync<T>(RawQuery.Create(sql, parameters), cancellationToken).ConfigureAwait(false);
        }
    }
}