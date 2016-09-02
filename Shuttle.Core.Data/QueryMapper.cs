using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class QueryMapper : IQueryMapper
    {
        private readonly IDatabaseGateway _databaseGateway;

        public QueryMapper(IDatabaseGateway databaseGateway)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");

            _databaseGateway = databaseGateway;
        }

        public MappedRow<T> MapRow<T>(IQuery query) where T : new()
        {
            var row = _databaseGateway.GetSingleRowUsing(query);

            return new MappedRow<T>(row, Map<T>(row));
        }

        private T Map<T>(DataRow row) where T : new()
        {
            var result = new T();
            var type = typeof (T);

            foreach (PropertyInfo pi in type.GetProperties())
            {
                var value = row[pi.Name];

                if (value == null)
                {
                    continue;
                }

                try
                {
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
            return _databaseGateway.GetRowsUsing(query).Select(row => new MappedRow<T>(row, Map<T>(row)));
        }

        public T MapObject<T>(IQuery query) where T : new()
        {
            return Map<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<T> MapObjects<T>(IQuery query) where T : new()
        {
            return _databaseGateway.GetRowsUsing(query).Select(row => Map<T>(row));
        }
    }
}