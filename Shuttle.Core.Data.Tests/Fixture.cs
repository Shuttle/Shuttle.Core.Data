using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public abstract class Fixture
    {
		protected static DataSource DefaultDataSource()
		{
			return new DataSource("Shuttle", new SqlDbDataParameterFactory());
		}
    }
}
