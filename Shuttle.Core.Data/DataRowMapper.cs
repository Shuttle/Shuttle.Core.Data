using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DataRowMapper : IDataRowMapper
    {
        public MappedRow<T> MapRow<T>(DataRow row) where T : new()
        {
            Guard.AgainstNull(row, nameof(row));

            return new MappedRow<T>(row, Map<T>(row));
        }

        public IEnumerable<MappedRow<T>> MapRows<T>(IEnumerable<DataRow> rows) where T : new()
        {
            return rows?.Select(row => new MappedRow<T>(row, Map<T>(row))).ToList() ?? new List<MappedRow<T>>();
        }

        public T MapObject<T>(DataRow row) where T : new()
        {
            return Map<T>(row);
        }

        public IEnumerable<T> MapObjects<T>(IEnumerable<DataRow> rows) where T : new()
        {
            return rows?.Select(Map<T>).ToList() ?? new List<T>();
        }

        public T MapValue<T>(DataRow row)
        {
            return MapRowValue<T>(row);
        }

        public IEnumerable<T> MapValues<T>(IEnumerable<DataRow> rows)
        {
            return rows?.Select(MapRowValue<T>).ToList() ?? new List<T>();
        }

        private T Map<T>(DataRow row) where T : new()
        {
            var result = new T();
            var type = typeof(T);

            foreach (var pi in type.GetProperties())
            {
                try
                {
                    var value = row.Table.Columns.Contains(pi.Name) ? row[pi.Name] : null;

                    if (value == null)
                    {
                        continue;
                    }

                    pi.SetValue(result, value, null);
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
                : (T)Convert.ChangeType(row[0], underlyingSystemType);
        }
    }
}