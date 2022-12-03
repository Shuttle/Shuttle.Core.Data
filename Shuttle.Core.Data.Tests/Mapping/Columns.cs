using System;
using System.Data;

namespace Shuttle.Core.Data.Tests;

public class Columns
{
    public static readonly MappedColumn<Guid> Id = new("Id", DbType.Guid);
}