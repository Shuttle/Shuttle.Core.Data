using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DataRowMapper : IDataRowMapper
{
    private readonly object _lock = new();
    private readonly Dictionary<Type, PropertyInfo[]> _properties = new();

    public MappedRow<T> MapRow<T>(DataRow row) where T : new()
    {
        Guard.AgainstNull(row);

        return new(row, Map<T>(GetPropertyInfo<T>(), row));
    }

    public IEnumerable<MappedRow<T>> MapRows<T>(IEnumerable<DataRow> rows) where T : new()
    {
        var properties = GetPropertyInfo<T>();

        return rows.Select(row => new MappedRow<T>(row, Map<T>(properties, row))).ToList();
    }

    public T? MapObject<T>(DataRow row) where T : new()
    {
        return Map<T>(GetPropertyInfo<T>(), row);
    }

    public IEnumerable<T> MapObjects<T>(IEnumerable<DataRow> rows) where T : new()
    {
        var properties = GetPropertyInfo<T>();

        return rows.Select(row => Map<T>(properties, row)).OfType<T>().ToList();
    }

    public T? MapValue<T>(DataRow row)
    {
        return MapRowValue<T>(row);
    }

    public IEnumerable<T> MapValues<T>(IEnumerable<DataRow> rows)
    {
        return rows.Select(MapRowValue<T>).OfType<T>().ToList();
    }

    public dynamic MapItem(DataRow row)
    {
        return DynamicMap(Guard.AgainstNull(row));
    }

    public IEnumerable<dynamic> MapItems(IEnumerable<DataRow> rows)
    {
        Guard.AgainstNull(rows);

        return rows.Any()
            ? rows.Select(row => DynamicMap(row)).ToList()
            : Enumerable.Empty<dynamic>();
    }

    private static dynamic DynamicMap(DataRow row)
    {
        var result = new ExpandoObject() as IDictionary<string, object>;

        foreach (DataColumn column in row.Table.Columns)
        {
            result.Add(column.ColumnName, row[column]);
        }

        return result;
    }

    private PropertyInfo[] GetPropertyInfo<T>()
    {
        lock (_lock)
        {
            var type = typeof(T);

            if (!_properties.ContainsKey(type))
            {
                _properties.Add(type, type.GetProperties());
            }

            return _properties[type];
        }
    }

    private static T? Map<T>(IEnumerable<PropertyInfo> properties, DataRow? row) where T : new()
    {
        if (row == null)
        {
            return default;
        }

        var result = new T();

        foreach (var pi in properties)
        {
            try
            {
                var value = row.Table.Columns.Contains(pi.Name) ? row[pi.Name] : null;

                if (value == null)
                {
                    continue;
                }

                if (value == DBNull.Value)
                {
                    value = null;
                }

                if (value != null ||
                    Nullable.GetUnderlyingType(pi.PropertyType) != null)
                {
                    pi.SetValue(result, value, null);
                }
            }
            catch
            {
                // ignored
            }
        }

        return result;
    }

    private static T? MapRowValue<T>(DataRow? row)
    {
        var underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        return row?[0] == null
            ? default
            : (T)Convert.ChangeType(row[0], underlyingSystemType);
    }
}