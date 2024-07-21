using System;
using System.Data;
using System.Data.Common;

namespace Shuttle.Core.Data
{
	public interface IDbCommandFactory
    {
        event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated;

        DbCommand Create(IDbConnection connection, IQuery query);
    }
}