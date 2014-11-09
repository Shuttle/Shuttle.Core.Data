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
		object RetrieveRawValueFrom(DataRow row);
		bool IsNullFor(DataRow row);
		IDbDataParameter CreateDataParameter(IDbDataParameterFactory factory, object value);
	}
}