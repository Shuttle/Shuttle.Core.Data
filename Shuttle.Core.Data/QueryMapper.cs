using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<MappedRow<T>> MapRow<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRow<T>(await _databaseGateway.GetRow(query, cancellationToken));
        }

        public async Task<IEnumerable<MappedRow<T>>> MapRows<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken()) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRows<T>(await _databaseGateway.GetRows(query, cancellationToken));
        }

        public async Task<T> MapObject<T>(IQuery query, CancellationToken cancellationToken= new CancellationToken()) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken);

            var columns = GetColumns(reader);

            if (await reader.ReadAsync(cancellationToken))
            {
                return Map<T>(GetPropertyInfo<T>(), reader, columns);
            }

            return default;
        }

        public async Task<IEnumerable<T>> MapObjects<T>(IQuery query, CancellationToken cancellationToken =  new CancellationToken()) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            var result = new List<T>();

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken);

            var columns = GetColumns(reader);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(Map<T>(GetPropertyInfo<T>(), reader, columns));
            }

            return result;
        }

        public async Task<T> MapValue<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                return MapRowValue<T>(reader);
            }

            return default;
        }

        public async Task<IEnumerable<T>> MapValues<T>(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            var result = new List<T>();

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var value = MapRowValue<T>(reader);

                if (value != null)
                {
                    result.Add(value);
                }
            }

            return result;
        }

        private static dynamic DynamicMap(IDataRecord record, Dictionary<string, int> columns)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;

            foreach (var pair in columns)
            {
                result.Add(pair.Key, record.GetValue(pair.Value));
            }

            return result;
        }

        public async Task<dynamic> MapItem(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken
            );
            var columns = GetColumns(reader);

            if (await reader.ReadAsync(cancellationToken))
            {
                return DynamicMap(reader, columns);
            }

            return default;
        }

        public async Task<IEnumerable<dynamic>> MapItems(IQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.AgainstNull(query, nameof(query));

            var result = new List<dynamic>();

            await using var reader = await _databaseGateway.GetReader(query, cancellationToken);

            var columns = GetColumns(reader);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(DynamicMap(reader, columns));
            }

            return result;
        }

        private IEnumerable<PropertyInfo> GetPropertyInfo<T>()
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

        private static T Map<T>(IEnumerable<PropertyInfo> properties, IDataRecord record, Dictionary<string, int> columns) where T : new()
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