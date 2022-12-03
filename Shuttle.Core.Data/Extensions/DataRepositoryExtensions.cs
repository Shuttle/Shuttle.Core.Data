using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public static class DataRepositoryExtensions
    {
        public static bool Contains<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters = null) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.Contains(RawQuery.Create(sql, parameters));
        }

        public static T FetchItem<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters = null) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItem(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<T> FetchItems<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters = null) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchItems(RawQuery.Create(sql, parameters));
        }

        public static MappedRow<T> FetchMappedRow<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters = null) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRow(RawQuery.Create(sql, parameters));
        }

        public static IEnumerable<MappedRow<T>> FetchMappedRows<T>(this IDataRepository<T> dataRepository, string sql, dynamic parameters = null) where T : class
        {
            Guard.AgainstNull(dataRepository, nameof(dataRepository));

            return dataRepository.FetchMappedRows(RawQuery.Create(sql, parameters));
        }
    }
}