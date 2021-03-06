﻿using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseContextFactoryFixture : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_database_context()
		{
			using (var context = GetDefaultDatabaseContextFactory().Create(DefaultConnectionStringName))
			{
				Assert.IsNotNull(context);
			}
		}

	    private IDatabaseContextFactory GetDefaultDatabaseContextFactory()
	    {
            return new DatabaseContextFactory(
	            GetConnectionConfigurationProvider(),
                new DbConnectionFactory(),
	            new DbCommandFactory(),
	            new ThreadStaticDatabaseContextCache());
	    }

        [Test]
		public void Should_be_able_to_get_an_existing_database_context()
        {
            var factory = GetDefaultDatabaseContextFactory();

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