using System.Data;

namespace Shuttle.Core.Data
{
	public interface IQuery
	{
		void Prepare(DataSource source, IDbCommand command);
		IQuery AddParameterValue(IMappedColumn column, object value);
	}
}