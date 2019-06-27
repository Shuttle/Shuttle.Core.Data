using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataRowMapper : IDataRowMapper
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Type, PropertyInfo[]> _properties = new Dictionary<Type, PropertyInfo[]>();

        public MappedRow<T> MapRow<T>(DataRow row) where T : new()
        {
            Guard.AgainstNull(row, nameof(row));

            return new MappedRow<T>(row, Map<T>(GetPropertyInfo<T>(), row));
        }

        public IEnumerable<MappedRow<T>> MapRows<T>(IEnumerable<DataRow> rows) where T : new()
        {
            var properties = GetPropertyInfo<T>();

            return rows?.Select(row => new MappedRow<T>(row, Map<T>(properties, row))).ToList() ??
                   new List<MappedRow<T>>();
        }

        public T MapObject<T>(DataRow row) where T : new()
        {
            return Map<T>(GetPropertyInfo<T>(), row);
        }

        public IEnumerable<T> MapObjects<T>(IEnumerable<DataRow> rows) where T : new()
        {
            var properties = GetPropertyInfo<T>();

            return rows?.Select(row => Map<T>(properties, row)).ToList() ?? new List<T>();
        }

        public T MapValue<T>(DataRow row)
        {
            return MapRowValue<T>(row);
        }

        public IEnumerable<T> MapValues<T>(IEnumerable<DataRow> rows)
        {
            return rows?.Select(MapRowValue<T>).ToList() ?? new List<T>();
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

        private T Map<T>(PropertyInfo[] properties, DataRow row) where T : new()
        {
            if (row == null)
            {
                return default(T);
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

        private static T MapRowValue<T>(DataRow row)
        {
            var underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return row?[0] == null
                ? default(T)
                : (T) Convert.ChangeType(row[0], underlyingSystemType);
        }
    }
}