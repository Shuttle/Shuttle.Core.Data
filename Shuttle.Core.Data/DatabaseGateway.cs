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
		private readonly IDatabaseContextCache _databaseContextCache;
		private readonly ILog _log;

		public DatabaseGateway(IDatabaseContextCache databaseContextCache)
		{
			Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

			_databaseContextCache = databaseContextCache;
			_log = Log.For(this);
		}

		public DataTable GetDataTable(IQuery query)
		{
            Guard.AgainstNull(query, nameof(query));

			using (var reader = GetReader(query))
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

		public IEnumerable<DataRow> GetRows(IQuery query)
		{
			return GetDataTable(query).Rows.Cast<DataRow>();
		}

		public DataRow GetRow(IQuery query)
		{
			var table = GetDataTable(query);

			if ((table == null) || (table.Rows.Count == 0))
			{
				return null;
			}

			return table.Rows[0];
		}

		public IDataReader GetReader(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
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

		public int Execute(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
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

		public T GetScalar<T>(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
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