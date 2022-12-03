using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class QueryMapperExtensions
    {
        public static dynamic MapItem(this IQueryMapper queryMapper, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapItem(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<dynamic> MapItems(this IQueryMapper queryMapper, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapItems(RawQuery.Create(sql, parameters));
        }

        public static T MapObject<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObject<T>(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<T> MapObjects<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapObjects<T>(RawQuery.Create(sql, parameters));
        }

        public static MappedRow<T> MapRow<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRow<T>(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<MappedRow<T>> MapRows<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null) where T : new()
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapRows<T>(RawQuery.Create(sql, parameters));
        }

        public static T MapValue<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValue<T>(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<T> MapValues<T>(this IQueryMapper queryMapper, string sql, dynamic parameters = null)
        {
            Guard.AgainstNull(queryMapper, nameof(queryMapper));

            return queryMapper.MapValues<T>(RawQuery.Create(sql, parameters));
        }
    }
}