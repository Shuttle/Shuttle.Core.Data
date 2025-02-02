using System.Collections.Generic;
using System.Data;

namespace Shuttle.Core.Data;

public interface IDataRowMapper
{
    dynamic MapItem(DataRow row);
    IEnumerable<dynamic> MapItems(IEnumerable<DataRow> rows);
    T? MapObject<T>(DataRow row) where T : new();
    IEnumerable<T> MapObjects<T>(IEnumerable<DataRow> rows) where T : new();
    MappedRow<T>? MapRow<T>(DataRow row) where T : new();
    IEnumerable<MappedRow<T>> MapRows<T>(IEnumerable<DataRow> rows) where T : new();
    T? MapValue<T>(DataRow row);
    IEnumerable<T> MapValues<T>(IEnumerable<DataRow> rows);
}

public interface IDataRowMapper<T> where T : class
{
    MappedRow<T> Map(DataRow row);
}