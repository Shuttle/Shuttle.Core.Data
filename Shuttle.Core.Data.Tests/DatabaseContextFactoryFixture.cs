﻿using System;
using System.Data;
using System.Threading;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
	public class DatabaseContextFactoryFixture : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_database_context()
		{
			using (var context = GetDatabaseContextFactory().Create(DefaultConnectionStringName))
			{
				Assert.IsNotNull(context);
			}
		}

        [Test]
		public void Should_be_able_to_get_an_existing_database_context()
        {
            var factory = GetDatabaseContextFactory();

            using (var context = factory.Create(DefaultConnectionStringName))
			using (var existingContext = factory.Create(DefaultConnectionStringName))
			{
				Assert.IsNotNull(context);
				Assert.IsNotNull(existingContext);

				Assert.AreSame(existingContext.Connection, context.Connection);
			}
		}

		[Test]
		public void Should_be_able_to_check_connection_availability()
		{
			var databaseContextFactory = new Mock<IDatabaseContextFactory>();

			Assert.That(databaseContextFactory.Object.IsAvailable(new CancellationToken(), 0, 0), Is.True);
			Assert.That(databaseContextFactory.Object.IsAvailable("name", new CancellationToken(), 0, 0), Is.True);
			Assert.That(databaseContextFactory.Object.IsAvailable("provider-name", new Mock<IDbConnection>().Object, new CancellationToken(), 0, 0), Is.True);
			Assert.That(databaseContextFactory.Object.IsAvailable("provider-name", "connection-string", new CancellationToken(), 0, 0), Is.True);

			databaseContextFactory.Setup(m => m.Create()).Throws(new Exception());
			databaseContextFactory.Setup(m => m.Create(It.IsAny<string>())).Throws(new Exception());
			databaseContextFactory.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<IDbConnection>())).Throws(new Exception());
			databaseContextFactory.Setup(m => m.Create(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

			Assert.That(databaseContextFactory.Object.IsAvailable(new CancellationToken(), 0, 0), Is.False);
			Assert.That(databaseContextFactory.Object.IsAvailable("name", new CancellationToken(), 0, 0), Is.False);
			Assert.That(databaseContextFactory.Object.IsAvailable("provider-name", new Mock<IDbConnection>().Object, new CancellationToken(), 0, 0), Is.False);
			Assert.That(databaseContextFactory.Object.IsAvailable("provider-name", "connection-string", new CancellationToken(), 0, 0), Is.False);
		}
	}
}