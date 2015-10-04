using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDbCommandFactory 
    {
        IDbCommand CreateCommandUsing(IDbConnection connection, IQuery query);
    }
}