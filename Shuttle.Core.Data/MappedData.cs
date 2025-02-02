using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class MappedData
{
    private readonly Dictionary<string, object> _data = new();

    public MappedData Add<T>(MappedRow<T> mappedRow)
    {
        var key = Key<T>();

        if (_data.ContainsKey(key))
        {
            _data.Remove(key);
        }

        _data.Add(key, new List<MappedRow<T>> { Guard.AgainstNull(mappedRow) });

        return this;
    }

    public MappedData Add<T>(IEnumerable<MappedRow<T>> mappedRows)
    {
        var key = Key<T>();

        if (_data.ContainsKey(key))
        {
            _data.Remove(key);
        }

        _data.Add(key, Guard.AgainstNull(mappedRows));

        return this;
    }

    private static string Key<T>()
    {
        return typeof(T).Name.ToLower();
    }

    public IEnumerable<MappedRow<T>> MappedRows<T>()
    {
        return _data.TryGetValue(Key<T>(), out var value) ? (IEnumerable<MappedRow<T>>)value : new List<MappedRow<T>>();
    }

    public IEnumerable<MappedRow<T>> MappedRows<T>(Func<MappedRow<T>, bool> func)
    {
        return MappedRows<T>().Where(Guard.AgainstNull(func).Invoke).ToList();
    }
}