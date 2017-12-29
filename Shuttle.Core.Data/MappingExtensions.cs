using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data
{
    public static class MappingExtensions
    {
        public static IEnumerable<MappedRow<T>> MappedRowsUsing<T>(this IEnumerable<DataRow> rows,
                                                                   IDataRowMapper<T> mapper) where T : class
        {
            var result = new List<MappedRow<T>>();

            foreach (var row in rows)
            {
                result.Add(mapper.Map(row));
            }

            return result;
        }
    }
}