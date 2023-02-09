using System.Data;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data
{
    public class DbCommandFactory : IDbCommandFactory
    {
		private readonly int _commandTimeout;

	    public DbCommandFactory(IOptions<DataAccessOptions> options)
	    {
		    Guard.AgainstNull(options, nameof(options));

		    _commandTimeout = Guard.AgainstNull(options.Value, nameof(options.Value)).CommandTimeout;
	    }

	    public IDbCommand Create(IDbConnection connection, IQuery query)
        {
            var command = Guard.AgainstNull(connection, nameof(connection)).CreateCommand();

        	command.CommandTimeout = _commandTimeout;

            Guard.AgainstNull(query, nameof(query)).Prepare(command);

            return command;
        }
    }
}