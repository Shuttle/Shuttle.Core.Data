using System;
using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDbCommandFactory
    {
        event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated;

        IDbCommand Create(IDbConnection connection, IQuery query);
    }
}