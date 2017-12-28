using System.Data.Common;
using System.Data.SqlClient;

namespace Shuttle.Core.Data
{
    public class DefaultSqlProviderFactory : DbProviderFactory
    {
        public override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
    }
}