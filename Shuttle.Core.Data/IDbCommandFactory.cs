using System;
using System.Data;
using System.Data.Common;

namespace Shuttle.Core.Data;

public interface IDbCommandFactory
{
    DbCommand Create(IDbConnection connection, IQuery query);
    event EventHandler<DbCommandCreatedEventArgs> DbCommandCreated;
}