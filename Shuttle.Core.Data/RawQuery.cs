using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class RawQuery : IQuery
    {
        private readonly Dictionary<IColumn, object> _parameterValues;
        private readonly string _sql;

        public RawQuery(string sql)
        {
            _sql = Guard.AgainstNullOrEmptyString(sql, nameof(sql));
            _parameterValues = new Dictionary<IColumn, object>();
        }

        public void Prepare(IDbCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            command.CommandText = _sql;
            command.CommandType = CommandType.Text;

            foreach (var pair in _parameterValues)
            {
                command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
            }
        }

        public IQuery AddParameterValue(IColumn column, object value)
        {
            Guard.AgainstNull(column, nameof(column));

            _parameterValues.Add(column, value);

            return this;
        }
    }
}