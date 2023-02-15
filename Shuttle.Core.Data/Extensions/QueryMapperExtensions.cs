using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class QueryMapperExtensions
    {
        public static async Task<dynamic> MapItem(this IQueryMapper queryMapper, string sql)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItem(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<dynamic> MapItem(this IQueryMapper queryMapper, string sql, dynamic parameters)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItem(RawQuery.Create(sql, parameters), CancellationToken.None);
        }
        
        public static async Task<dynamic> MapItem(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItem(RawQuery.Create(sql), cancellationToken);
        }
        
        public static async Task<dynamic> MapItem(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItem(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<dynamic>> MapItems(this IQueryMapper queryMapper, string sql)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItems(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<dynamic>> MapItems(this IQueryMapper queryMapper, string sql, dynamic parameters)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItems(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<dynamic>> MapItems(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItems(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<dynamic>> MapItems(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapItems(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> MapObject<T>(this IQueryMapper queryMapper, string sql) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObject<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<T> MapObject<T>(this IQueryMapper queryMapper, string sql, dynamic parameters) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObject<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<T> MapObject<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObject<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> MapObject<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObject<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapObjects<T>(this IQueryMapper queryMapper, string sql) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjects<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> MapObjects<T>(this IQueryMapper queryMapper, string sql, dynamic parameters) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjects<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> MapObjects<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjects<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapObjects<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapObjects<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<MappedRow<T>> MapRow<T>(this IQueryMapper queryMapper, string sql) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRow<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<MappedRow<T>> MapRow<T>(this IQueryMapper queryMapper, string sql, dynamic parameters) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRow<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<MappedRow<T>> MapRow<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRow<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<MappedRow<T>> MapRow<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRow<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRows<T>(this IQueryMapper queryMapper, string sql) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRows<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRows<T>(this IQueryMapper queryMapper, string sql, dynamic parameters) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRows<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRows<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRows<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<MappedRow<T>>> MapRows<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapRows<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<T> MapValue<T>(this IQueryMapper queryMapper, string sql)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValue<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<T> MapValue<T>(this IQueryMapper queryMapper, string sql, dynamic parameters)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValue<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<T> MapValue<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValue<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<T> MapValue<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValue<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapValues<T>(this IQueryMapper queryMapper, string sql)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValues<T>(RawQuery.Create(sql), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> MapValues<T>(this IQueryMapper queryMapper, string sql, dynamic parameters)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValues<T>(RawQuery.Create(sql, parameters), CancellationToken.None);
        }

        public static async Task<IEnumerable<T>> MapValues<T>(this IQueryMapper queryMapper, string sql, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValues<T>(RawQuery.Create(sql), cancellationToken);
        }

        public static async Task<IEnumerable<T>> MapValues<T>(this IQueryMapper queryMapper, string sql, dynamic parameters, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return await queryMapper.MapValues<T>(RawQuery.Create(sql, parameters), cancellationToken);
        }
    }
}