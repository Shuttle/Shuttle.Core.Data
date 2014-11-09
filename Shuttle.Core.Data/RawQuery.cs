using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class RawQuery : IQuery
	{
		private readonly Dictionary<IMappedColumn, object> parameterValues;
		private readonly string sql;

		public RawQuery(string sql)
		{
			this.sql = sql;
			parameterValues = new Dictionary<IMappedColumn, object>();
		}

		public void Prepare(DataSource source, IDbCommand command)
		{
			Guard.AgainstNull(source, "source");
			Guard.AgainstNull(command, "command");

			command.CommandText = sql;
			command.CommandType = CommandType.Text;

			foreach (var pair in parameterValues)
			{
				command.Parameters.Add(pair.Key.CreateDataParameter(source.DbDataParameterFactory, pair.Value));
			}
		}

		public IQuery AddParameterValue(IMappedColumn column, object value)
		{
			parameterValues.Add(column, value);

			return this;
		}

		public static IQuery Create(string sql, params object[] args)
		{
			return new RawQuery(string.Format(sql, args));
		}
	}
}