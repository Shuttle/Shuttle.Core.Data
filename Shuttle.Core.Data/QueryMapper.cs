using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class QueryMapper : IQueryMapper
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IDataRowMapper _dataRowMapper;
    private readonly object _lock = new();
    private readonly Dictionary<Type, PropertyInfo[]> _properties = new();

    public QueryMapper(IDatabaseContextService databaseContextService, IDataRowMapper dataRowMapper)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _dataRowMapper = Guard.AgainstNull(dataRowMapper);
    }

    public async Task<MappedRow<T>?> MapRowAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new()
    {
        var result = await _databaseContextService.Active.GetRowAsync(Guard.AgainstNull(query), cancellationToken);

        return result == null ? null : _dataRowMapper.MapRow<T>(result);
    }

    public async Task<IEnumerable<MappedRow<T>>> MapRowsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new()
    {
        return _dataRowMapper.MapRows<T>(await _databaseContextService.Active.GetRowsAsync(Guard.AgainstNull(query), cancellationToken));
    }

    public async Task<T?> MapObjectAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new()
    {
        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

        var columns = GetColumns(reader);

        if (await reader.ReadAsync(cancellationToken))
        {
            return Map<T>(GetPropertyInfo<T>(), reader, columns);
        }

        return default;
    }

    public async Task<IEnumerable<T>> MapObjectsAsync<T>(IQuery query, CancellationToken cancellationToken = default) where T : new()
    {
        Guard.AgainstNull(query);

        List<T> result = new();

        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

        var columns = GetColumns(reader);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(Map<T>(GetPropertyInfo<T>(), reader, columns));
        }

        return result;
    }

    public async Task<T?> MapValueAsync<T>(IQuery query, CancellationToken cancellationToken = default)
    {
        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            return MapRowValue<T>(reader);
        }

        return default;
    }

    public async Task<IEnumerable<T>> MapValuesAsync<T>(IQuery query, CancellationToken cancellationToken = default)
    {
        List<T> result = new();

        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

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

    public async Task<dynamic?> MapItemAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

        var columns = GetColumns(reader);

        if (await reader.ReadAsync(cancellationToken))
        {
            return DynamicMap(reader, columns);
        }

        return default;
    }

    public async Task<IEnumerable<dynamic>> MapItemsAsync(IQuery query, CancellationToken cancellationToken = default)
    {
        List<dynamic> result = new();

        await using var reader = await _databaseContextService.Active.GetReaderAsync(Guard.AgainstNull(query), cancellationToken);

        var columns = GetColumns(reader);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(DynamicMap(reader, columns));
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

    private static Dictionary<string, int> GetColumns(IDataRecord record)
    {
        return Enumerable.Range(0, record.FieldCount).ToDictionary(record.GetName, item => item);
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
        T result = new();

        foreach (var pi in properties)
        {
            try
            {
                var value = columns.TryGetValue(pi.Name, out var column) ? record.GetValue(column) : null;

                if (value == null)
                {
                    continue;
                }

                if (value == DBNull.Value)
                {
                    value = null;
                }

                if (value != null || Nullable.GetUnderlyingType(pi.PropertyType) != null)
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

    private static T MapRowValue<T>(DbDataReader reader)
    {
        return (T)Convert.ChangeType(reader.GetValue(0), Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }
}