using System.Data;

namespace Shuttle.Core.Data
{
    public interface IDbConnectionFactory 
    {
        IDbConnection CreateConnection(DataSource source);
    }
}