using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
	public class DatabaseGateway : IDatabaseGateway
	{
		private readonly ILog _log;

		public DatabaseGateway()
		{
			_log = Log.For(this);
		}

		public DataTable GetDataTableFor(IQuery query)
		{
			using (var reader = GetReaderUsing(query))
			{
				var results = new DataTable();

				if (reader != null)
				{
					results.Load(reader);
				}

				return results;
			}
		}

		private void Trace(IDbCommand command)
		{
			var parameters = new StringBuilder();

			foreach (IDataParameter parameter in command.Parameters)
			{
				parameters.AppendFormat(" / {0} = {1}", parameter.ParameterName, parameter.Value);
			}

			_log.Trace(string.Format("{0} {1}", command.CommandText, parameters));
		}

		public IEnumerable<DataRow> GetRowsUsing(IQuery query)
		{
			return GetDataTableFor(query).Rows.Cast<DataRow>();
		}

		public DataRow GetSingleRowUsing(IQuery query)
		{
			var table = GetDataTableFor(query);

			if ((table == null) || (table.Rows.Count == 0))
			{
				return null;
			}

			return table.Rows[0];
		}

		public IDataReader GetReaderUsing(IQuery query)
		{
			using (var command = DatabaseContext.Current.CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

				return command.ExecuteReader();
			}
		}

		public int ExecuteUsing(IQuery query)
		{
			using (var command = DatabaseContext.Current.CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

				return command.ExecuteNonQuery();
			}
		}

		public T GetScalarUsing<T>(IQuery query)
		{
			using (var command = DatabaseContext.Current.CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

				var scalar = command.ExecuteScalar();

				return (scalar != null && scalar != DBNull.Value) ? (T)scalar : default(T);
			}
		}
	}
}