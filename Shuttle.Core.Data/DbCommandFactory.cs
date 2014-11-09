using System.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data
{
    public class DbCommandFactory : IDbCommandFactory
    {
		private static readonly int commandTimeout = ConfigurationItem<int>.ReadSetting("Shuttle.Core.Data.DbCommandFactory.CommandTimeout", 15).GetValue();

        public IDbCommand CreateCommandUsing(DataSource source, IDbConnection connection, IQuery query)
        {
            var command = connection.CreateCommand();

        	command.CommandTimeout = commandTimeout;
            query.Prepare(source, command);

            return command;
        }
    }
}