using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public abstract class Fixture
    {
	    protected static string DefaultConnectionStringName = "Shuttle";
		protected static string DefaultProviderName = "System.Data.SqlClient";
		protected static string DefaultConnectionString = "Data Source=.;Initial Catalog=Shuttle;Integrated Security=SSPI";

	    protected IDatabaseConnection GetDatabaseConnection()
		{
			return new DatabaseConnectionFactory(new DbConnectionFactory(), new DbCommandFactory()).Create(DefaultConnectionStringName);
		}
    }
}
