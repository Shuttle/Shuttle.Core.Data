using System.Data;

namespace Shuttle.Core.Data
{
	public interface IQuery
	{
		void Prepare(IDbCommand command);
    }
}