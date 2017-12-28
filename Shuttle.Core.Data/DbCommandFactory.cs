using System.Data;
using Shuttle.Core.Configuration;

namespace Shuttle.Core.Data
{
    public class DbCommandFactory : IDbCommandFactory
    {
		private readonly int _commandTimeout;

	    public DbCommandFactory()
			: this(ConfigurationItem<int>.ReadSetting("Shuttle.Core.Data.DbCommandFactory.CommandTimeout", 15).GetValue())
	    {
	    }

	    private DbCommandFactory(int commandTimeout)
	    {
		    _commandTimeout = commandTimeout;
	    }

	    public IDbCommand CreateCommandUsing(IDbConnection connection, IQuery query)
        {
            var command = connection.CreateCommand();

        	command.CommandTimeout = _commandTimeout;
            query.Prepare(command);

            return command;
        }
    }
}