using System;
using System.Data;

namespace Shuttle.Core.Data.Tests.DataAccess;

public class Columns
{
    public static readonly Column<Guid> Id = new("Id", DbType.Guid);
    public static readonly Column<int> Age = new("Age", DbType.Int32);
    public static readonly Column<string> Name = new("Name", DbType.String);
}