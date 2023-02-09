using System.Data;

namespace Shuttle.Core.Data
{
	public interface IMappedColumn
	{
		string ColumnName { get; }
		DbType DbType { get; }
		int? Size { get; }
		byte? Precision { get; }
		byte? Scale { get; }
		string FlattenedColumnName();
		object RetrieveRawValueFrom(dynamic instance);
		object RetrieveRawValueFrom(DataRow row);
		object RetrieveRawValueFrom(IDataRecord record);
		bool IsNullFor(dynamic instance);
		bool IsNullFor(DataRow row);
		bool IsNullFor(IDataRecord record);
		IDbDataParameter CreateDataParameter(IDbCommand command, object value);
	}
}