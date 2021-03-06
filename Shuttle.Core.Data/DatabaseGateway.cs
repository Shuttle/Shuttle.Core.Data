using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;

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
            Guard.AgainstNull(query, nameof(query));

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

			_log.Trace($"{command.CommandText} {parameters}");
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
		    Guard.AgainstNull(query, nameof(query));

			using (var command = GuardedDatabaseContext().CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

			    try
			    {
			        return command.ExecuteReader();
			    }
			    catch (Exception ex)
			    {
			        _log.Error(ex.Message);

			        if (!Log.IsTraceEnabled)
			        {
			            Trace(command);
			        }

			        throw;
			    }
			}
		}

		private static IDatabaseContext GuardedDatabaseContext()
		{
			var result = DatabaseContext.Current;

			if (result == null)
			{
				throw new InvalidOperationException(Resources.DatabaseContextMissing);
			}

			return result;
		}

		public int ExecuteUsing(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = GuardedDatabaseContext().CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

			    try
			    {
			        return command.ExecuteNonQuery();
			    }
			    catch (Exception ex)
			    {
			        _log.Error(ex.Message);

			        if (!Log.IsTraceEnabled)
			        {
			            Trace(command);
			        }

			        throw;
			    }
            }
		}

		public T GetScalarUsing<T>(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = GuardedDatabaseContext().CreateCommandToExecute(query))
			{
				if (Log.IsTraceEnabled)
				{
					Trace(command);
				}

			    object scalar;

			    try
                {
			        scalar = command.ExecuteScalar();
			    }
			    catch (Exception ex)
			    {
			        _log.Error(ex.Message);

			        if (!Log.IsTraceEnabled)
			        {
			            Trace(command);
			        }

			        throw;
			    }

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default(T);
			}
		}
	}
}