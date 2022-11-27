using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class QueryMapper : IQueryMapper
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Type, PropertyInfo[]> _properties = new Dictionary<Type, PropertyInfo[]>();

        private readonly IDatabaseGateway _databaseGateway;
        private readonly IDataRowMapper _dataRowMapper;

        public QueryMapper(IDatabaseGateway databaseGateway, IDataRowMapper dataRowMapper)
        {
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _dataRowMapper = Guard.AgainstNull(dataRowMapper, nameof(dataRowMapper));
        }

        public MappedRow<T> MapRow<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRow<T>(_databaseGateway.GetRow(query));
        }

        public IEnumerable<MappedRow<T>> MapRows<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRows<T>(_databaseGateway.GetRows(query));
        }

        public T MapObject<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            using (var reader = _databaseGateway.GetReader(query))
            {
                var columns = GetColumns(reader);

                while (reader.Read())
                {
                    return Map<T>(GetPropertyInfo<T>(), reader, columns);
                }
            }

            return default;
        }

        public IEnumerable<T> MapObjects<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            var result = new List<T>();

            using (var reader = _databaseGateway.GetReader(query))
            {
                var columns = GetColumns(reader);

                while (reader.Read())
                {
                    result.Add(Map<T>(GetPropertyInfo<T>(), reader, columns));
                }
            }

            return result;
        }

        public T MapValue<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            using (var reader = _databaseGateway.GetReader(query))
            {
                var columns = GetColumns(reader);

                while (reader.Read())
                {
                    return MapRowValue<T>(reader);
                }
            }

            return default;
        }

        public IEnumerable<T> MapValues<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            var result = new List<T>();

            using (var reader = _databaseGateway.GetReader(query))
            {
                var columns = GetColumns(reader);

                while (reader.Read())
                {
                    var value = MapRowValue<T>(reader);

                    if (value != null)
                    {
                        result.Add(value);
                    }
                }
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

        private T Map<T>(IEnumerable<PropertyInfo> properties, IDataRecord record, Dictionary<string, int> columns) where T : new()
        {
            var result = new T();

            foreach (var pi in properties)
            {
                try
                {
                    var value = columns.ContainsKey(pi.Name) ? record.GetValue(columns[pi.Name]) : null;

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

        private static Dictionary<string, int> GetColumns(IDataRecord record) 
        {
            return Enumerable.Range(0, record.FieldCount).ToDictionary(item => record.GetName(item) ?? $"__missing_column_name:{item}", item => item);
        }

        private static T MapRowValue<T>(IDataReader dataReader)
        {
            var underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            var value = dataReader.GetValue(0);

            return value == null
                ? default
                : (T)Convert.ChangeType(value, underlyingSystemType);
        }
    }
}