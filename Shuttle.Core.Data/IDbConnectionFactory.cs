using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDbConnectionFactory 
    {
	    IDbConnection CreateConnection(string providerName, string connectionString);
    }
}