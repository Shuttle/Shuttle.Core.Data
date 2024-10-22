using System.Data;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public class MappedRow<T>
{
    public MappedRow(DataRow row, T? result)
    {
        Row = Guard.AgainstNull(row);
        Result = result;
    }

    public T? Result { get; }
    public DataRow Row { get; }
}