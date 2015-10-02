using System;
using System.Collections.Generic;

namespace Shuttle.Core.Data
{
    public class MappedData
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public MappedData Add<T>(MappedRow<T> mappedRow)
        {
            var key = Key<T>();

            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }

            _data.Add(key, new List<MappedRow<T>> {mappedRow});

            return this;
        }

        public MappedData Add<T>(IEnumerable<MappedRow<T>> mappedRows)
        {
            var key = Key<T>();

            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }

            _data.Add(key, mappedRows);

            return this;
        }

        public IEnumerable<MappedRow<T>> MappedRows<T>()
        {
            var key = Key<T>();

            if (_data.ContainsKey(key))
            {
                return (IEnumerable<MappedRow<T>>) _data[key];
            }

            return new List<MappedRow<T>>();
        }

        private static string Key<T>()
        {
            return typeof (T).Name.ToLower();
        }

        public IEnumerable<MappedRow<T>> MappedRows<T>(Func<MappedRow<T>, bool> func)
        {
            var result = new List<MappedRow<T>>();

            foreach (var mappedRow in MappedRows<T>())
            {
                if (func.Invoke(mappedRow))
                {
                    result.Add(mappedRow);
                }
            }

            return result;
        }
    }
}