using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseContextFactoryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_database_context()
		{
			var factory = DatabaseContextFactory.Default();
			using (var context = factory.Create(DefaultConnectionStringName))
			{
				Assert.IsNotNull(context);
			}
		}

		[Test]
		public void Should_be_able_to_get_an_existing_database_context()
		{
			var factory = DatabaseContextFactory.Default();

			using (var context = factory.Create(DefaultConnectionStringName))
			using (var existingContext = factory.Create(DefaultConnectionStringName))
			{
				Assert.IsNotNull(context);
				Assert.IsNotNull(existingContext);

				Assert.AreSame(existingContext.Connection, context.Connection);
			}
		}
	}
}