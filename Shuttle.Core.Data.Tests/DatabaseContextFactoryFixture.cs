using System;
using System.Data.Common;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DatabaseContextFactoryFixture : Fixture
{
    [Test]
    public void Should_be_able_to_create_a_database_context()
    {
        using (var context = DatabaseContextFactory.Create(DefaultConnectionStringName))
        {
            Assert.IsNotNull(context);
        }
    }

    [Test]
    public void Should_not_not_be_able_to_create_another_context_with_the_same_name_as_an_existing_context()
    {
        var factory = DatabaseContextFactory;

        using (factory.Create(DefaultConnectionStringName))
        {
            Assert.That(()=> factory.Create(DefaultConnectionStringName), Throws.InvalidOperationException);
        }
    }

    [Test]
    public void Should_be_able_to_check_connection_availability()
    {
        var databaseContextFactory = new Mock<IDatabaseContextFactory>();

        Assert.That(databaseContextFactory.Object.IsAvailable(new CancellationToken(), 0, 0), Is.True);
        Assert.That(databaseContextFactory.Object.IsAvailable("name", new CancellationToken(), 0, 0), Is.True);

        databaseContextFactory.Setup(m => m.Create()).Throws(new Exception());
        databaseContextFactory.Setup(m => m.Create(It.IsAny<string>())).Throws(new Exception());

        Assert.That(databaseContextFactory.Object.IsAvailable(new CancellationToken(), 0, 0), Is.False);
        Assert.That(databaseContextFactory.Object.IsAvailable("name", new CancellationToken(), 0, 0), Is.False);
    }
}