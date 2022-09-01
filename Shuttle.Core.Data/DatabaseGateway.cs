using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
	public class DatabaseGateway : IDatabaseGateway
	{
		private readonly IDatabaseContextCache _databaseContextCache;
		
		public DatabaseGateway(IDatabaseContextCache databaseContextCache)
		{
			Guard.AgainstNull(databaseContextCache, nameof(databaseContextCache));

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

		public event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated = delegate
		{
		};

		public IDataReader GetReader(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
			{
				DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

			    return command.ExecuteReader();
			}
		}

		public int Execute(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
			{
				DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

				return command.ExecuteNonQuery();
            }
		}

		public T GetScalar<T>(IQuery query)
		{
		    Guard.AgainstNull(query, nameof(query));

			using (var command = _databaseContextCache.Current.CreateCommandToExecute(query))
			{
				DbCommandCreated.Invoke(this, new DbCommandCreatedEventArgs(command));

				var scalar = command.ExecuteScalar();

                return scalar != null && scalar != DBNull.Value ? (T)scalar : default(T);
			}
		}
	}
}