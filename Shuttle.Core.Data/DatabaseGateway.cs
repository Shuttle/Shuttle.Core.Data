using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DatabaseGateway : IDatabaseGateway
	{
		private readonly ILogger<DatabaseGateway> _logger;
		private readonly IDatabaseContextCache _databaseContextCache;
		
		public DatabaseGateway(ILogger<DatabaseGateway> logger, IDatabaseContextCache databaseContextCache)
		{
			Guard.AgainstNull(logger, nameof(logger));
			Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

			_logger = logger;
			_databaseContextCache = databaseContextCache;
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

			_logger.LogTrace($"{command.CommandText} {parameters}");
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
				if (_logger.IsEnabled(LogLevel.Trace))
				{
					Trace(command);
				}

			    try
			    {
			        return command.ExecuteReader();
			    }
			    catch (Exception ex)
			    {
				    (_logger).LogError(ex.Message);

					if (!_logger.IsEnabled(LogLevel.Trace))
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
				if (_logger.IsEnabled(LogLevel.Trace))
				{
					Trace(command);
				}

			    try
			    {
			        return command.ExecuteNonQuery();
			    }
			    catch (Exception ex)
			    {
			        _logger.LogError(ex.Message);

					if (!_logger.IsEnabled(LogLevel.Trace))
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
				if (_logger.IsEnabled(LogLevel.Trace))
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
			        _logger.LogError(ex.Message);

					if (!_logger.IsEnabled(LogLevel.Trace))
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