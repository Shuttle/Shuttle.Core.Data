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
		private readonly IDatabaseConnectionCache _databaseConnectionCache;

		private readonly ILog _log;

		public static IDatabaseGateway Default()
		{
			return new DatabaseGateway(new ThreadStaticDatabaseConnectionCache());
		}

		public DatabaseGateway(IDatabaseConnectionCache databaseConnectionCache)
		{
			Guard.AgainstNull(databaseConnectionCache, "databaseConnectionCache");

			_databaseConnectionCache = databaseConnectionCache;

			_log = Log.For(this);
		}

		public DataTable GetDataTableFor(DataSource source, IQuery query)
		{
			using (var reader = GetReaderUsing(source, query))
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

		public IEnumerable<DataRow> GetRowsUsing(DataSource source, IQuery query)
		{
			return GetDataTableFor(source, query).Rows.Cast<DataRow>();
		}

		public DataRow GetSingleRowUsing(DataSource source, IQuery query)
		{
			var table = GetDataTableFor(source, query);

			if ((table == null) || (table.Rows.Count == 0))
			{
				return null;
			}

			return table.Rows[0];
		}

		public IDataReader GetReaderUsing(DataSource source, IQuery query)
		{
			using (var command = _databaseConnectionCache.Get(source).CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

				return command.ExecuteReader();
			}
		}

		public int ExecuteUsing(DataSource source, IQuery query)
		{
			using (var command = _databaseConnectionCache.Get(source).CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

				return command.ExecuteNonQuery();
			}
		}

		public T GetScalarUsing<T>(DataSource source, IQuery query)
		{
			using (var command = _databaseConnectionCache.Get(source).CreateCommandToExecute(query))
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