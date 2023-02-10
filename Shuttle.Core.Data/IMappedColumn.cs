using System.Data;

namespace Shuttle.Core.Data
{
	public interface IColumn
	{
		string Name { get; }
		DbType DbType { get; }
		int? Size { get; }
		byte? Precision { get; }
		byte? Scale { get; }
		string FlattenedName();
		object RawValue(dynamic instance);
		object RawValue(DataRow row);
		object RawValue(IDataRecord record);
		bool IsNull(dynamic instance);
		bool IsNull(DataRow row);
		bool IsNull(IDataRecord record);
		IDbDataParameter CreateDataParameter(IDbCommand command, object value);
	}
}