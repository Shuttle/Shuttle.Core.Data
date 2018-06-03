using System.Collections.Generic;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class QueryMapper : IQueryMapper
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IDataRowMapper _dataRowMapper;

        public QueryMapper(IDatabaseGateway databaseGateway, IDataRowMapper dataRowMapper)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(dataRowMapper, nameof(dataRowMapper));

            _databaseGateway = databaseGateway;
            _dataRowMapper = dataRowMapper;
        }

        public MappedRow<T> MapRow<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRow<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<MappedRow<T>> MapRows<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapRows<T>(_databaseGateway.GetRowsUsing(query));
        }

        public T MapObject<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapObject<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<T> MapObjects<T>(IQuery query) where T : new()
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapObjects<T>(_databaseGateway.GetRowsUsing(query));
        }

        public T MapValue<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapValue<T>(_databaseGateway.GetSingleRowUsing(query));
        }

        public IEnumerable<T> MapValues<T>(IQuery query)
        {
            Guard.AgainstNull(query, nameof(query));

            return _dataRowMapper.MapValues<T>(_databaseGateway.GetRowsUsing(query));
        }
    }
}