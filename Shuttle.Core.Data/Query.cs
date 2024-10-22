using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class Query : IQuery
{
    private readonly string _commandText;
    private readonly CommandType _commandType;
    private readonly Dictionary<IColumn, object> _parameterValues;

    public Query(string commandText, CommandType commandType = CommandType.Text)
    {
        _commandText = Guard.AgainstNullOrEmptyString(commandText);
        _commandType = commandType;
        _parameterValues = new();
    }

    public void Prepare(IDbCommand command)
    {
        Guard.AgainstNull(command);

        command.CommandText = _commandText;
        command.CommandType = _commandType;

        foreach (var pair in _parameterValues)
        {
            command.Parameters.Add(pair.Key.CreateDataParameter(command, pair.Value));
        }
    }

    public IQuery AddParameter(IColumn column, object? value)
    {
        Guard.AgainstNull(column);

        _parameterValues.Add(column, value ?? DBNull.Value);

        return this;
    }
}