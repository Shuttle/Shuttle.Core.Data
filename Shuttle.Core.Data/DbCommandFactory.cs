using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class DbCommandFactory : IDbCommandFactory
{
    private readonly int _commandTimeout;

    public DbCommandFactory(IOptions<DataAccessOptions> options)
    {
        Guard.AgainstNull(options);

        _commandTimeout = Guard.AgainstNull(options.Value).CommandTimeout;
    }

    public event EventHandler<DbCommandCreatedEventArgs>? DbCommandCreated;

    public DbCommand Create(IDbConnection connection, IQuery query)
    {
        var command = Guard.AgainstNull(connection).CreateCommand();

        command.CommandTimeout = _commandTimeout;

        Guard.AgainstNull(query).Prepare(command);

        DbCommandCreated?.Invoke(this, new(command));

        return (DbCommand)command;
    }
}