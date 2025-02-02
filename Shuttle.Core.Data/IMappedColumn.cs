using System.Data;

namespace Shuttle.Core.Data;

public interface IColumn
{
    DbType DbType { get; }
    string Name { get; }
    byte? Precision { get; }
    byte? Scale { get; }
    int? Size { get; }
    IDbDataParameter CreateDataParameter(IDbCommand command, object? value);
    string FlattenedName();
    bool IsNull(dynamic instance);
    bool IsNull(DataRow row);
    bool IsNull(IDataRecord record);
    object? RawValue(dynamic instance);
    object? RawValue(DataRow row);
    object? RawValue(IDataRecord record);
}