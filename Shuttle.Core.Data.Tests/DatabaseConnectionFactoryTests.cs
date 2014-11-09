using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseConnectionFactoryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_database_connection()
		{
			var factory = DatabaseConnectionFactory.Default();
			using (var connection = factory.Create(DefaultDataSource()))
			{
				Assert.IsNotNull(connection);
			}
		}

		[Test]
		public void Should_be_able_to_get_an_existing_database_connection()
		{
			var factory = DatabaseConnectionFactory.Default();

			using (var connection = factory.Create(DefaultDataSource()))
			using (var existingConnection = factory.Create(DefaultDataSource()))
			{
				Assert.IsNotNull(connection);
				Assert.IsNotNull(existingConnection);

				Assert.AreSame(existingConnection.Connection, connection.Connection);
			}
		}
	}
}