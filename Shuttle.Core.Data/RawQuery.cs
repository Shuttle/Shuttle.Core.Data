using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class RawQuery : IQueryParameter
	{
		private readonly Dictionary<IMappedColumn, object> _parameterValues;
		private readonly string sql;

		public RawQuery(string sql)
		{
			this.sql = sql;
			_parameterValues = new Dictionary<IMappedColumn, object>();
		}

		public void Prepare(IDbCommand command)
		{
			Guard.AgainstNull(command, "command");

			command.CommandText = sql;
			command.CommandType = CommandType.Text;

			foreach (var pair in _parameterValues)
			{
				command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
			}
		}

        public IQueryParameter AddParameterValue(IMappedColumn column, object value)
		{
			_parameterValues.Add(column, value);

			return this;
		}

        public static IQueryParameter Create(string sql, params object[] args)
		{
			return new RawQuery(string.Format(sql, args));
		}
	}
}