using System.Data;
using Microsoft.Extensions.Options;

namespace Shuttle.Core.Data
{
    public class DbCommandFactory : IDbCommandFactory
    {
		private readonly int _commandTimeout;

	    public DbCommandFactory(IOptions<CommandSettings> options)
	    {
		    _commandTimeout = options.Value.CommandTimeout;
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