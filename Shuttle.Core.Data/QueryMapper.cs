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
        private readonly IDatabaseGateway _databaseGateway;

        public QueryMapper(IDatabaseGateway databaseGateway)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));

            _databaseGateway = databaseGateway;
        }

        public MappedRow<T> MapRow<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            var row = _databaseGateway.GetSingleRowUsing(query);

            return new MappedRow<T>(row, Map<T>(row));
        }

        private T Map<T>(DataRow row) where T : new()
        {
            var result = new T();
            var type = typeof (T);

            foreach (PropertyInfo pi in type.GetProperties())
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
                }
            }

            return result;
        }

        public IEnumerable<MappedRow<T>> MapRows<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _databaseGateway.GetRowsUsing(query).Select(row => new MappedRow<T>(row, Map<T>(row)));
        }

        public T MapObject<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return Map<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<T> MapObjects<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _databaseGateway.GetRowsUsing(query).Select(Map<T>);
        }

        public T MapValue<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            return MapRowValue<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<T> MapValues<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            return _databaseGateway.GetRowsUsing(query).Select(MapRowValue<T>);
        }

        private T MapRowValue<T>(DataRow row)
        {
            var underlyingSystemType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return row[0] == null
                        ? default(T)
                        : (T)Convert.ChangeType(row[0], underlyingSystemType);
        }
    }
}