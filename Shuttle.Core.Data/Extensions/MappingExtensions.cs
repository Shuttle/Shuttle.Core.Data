using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class MappingExtensions
{
    public static IEnumerable<MappedRow<T>> MappedRowsUsing<T>(this IEnumerable<DataRow> rows, IDataRowMapper<T> mapper) where T : class
    {
        Guard.AgainstNull(mapper);

        return Guard.AgainstNull(rows).Select(mapper.Map).ToList();
    }
}