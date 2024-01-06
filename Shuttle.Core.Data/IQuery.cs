using System.Data;

namespace Shuttle.Core.Data
{
	public interface IQuery
	{
		void Prepare(IDbCommand command);
		IQuery AddParameter(IColumn column, object value);
    }
}