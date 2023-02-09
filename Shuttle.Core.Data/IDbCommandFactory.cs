using System.Data;

namespace Shuttle.Core.Data
{
	public interface IDbCommandFactory 
    {
        IDbCommand Create(IDbConnection connection, IQuery query);
    }
}