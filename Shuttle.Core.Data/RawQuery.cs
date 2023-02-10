using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class RawQuery : IQueryParameter
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

        public IQueryParameter AddParameterValue(IColumn column, object value)
        {
            Guard.AgainstNull(column, nameof(column));

            _parameterValues.Add(column, value);

            return this;
        }

        public static IQueryParameter Create(string sql, params object[] args)
        {
            return new RawQuery(args != null && args.Length > 0 ? string.Format(sql, args) : sql);
        }

        public static IQueryParameter Create(string sql, dynamic parameters)
        {
            var result = new RawQuery(sql);

            if (parameters != null)
            {
                foreach (var pi in ((object)parameters).GetType().GetProperties())
                {
                    try
                    {
                        result.AddParameterValue(new Column(pi.Name, pi.PropertyType, Column.GetDbType(pi.PropertyType)), pi.GetValue(parameters));
                    }
                    catch
                    {
                        throw new InvalidOperationException(string.Format(Resources.DynamicGetValueException, pi.Name));
                    }
                }
            }

            return result;
        }
    }
}