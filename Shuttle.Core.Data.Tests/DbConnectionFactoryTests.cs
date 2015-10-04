using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DbConnectionFactoryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_valid_connection()
		{
			var factory = new DbConnectionFactory();

			using (var connection = factory.CreateConnection(DefaultProviderName, DefaultConnectionString))
			{
				Assert.IsNotNull(connection);
			}
		}
	}
}