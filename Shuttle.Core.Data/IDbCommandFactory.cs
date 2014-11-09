using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDbCommandFactory 
    {
        IDbCommand CreateCommandUsing(DataSource source, IDbConnection connection, IQuery query);
    }
}