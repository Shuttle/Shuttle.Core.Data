using System.Data.Common;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class DatabaseContextServiceFixture : Fixture
{
    [Test]
    public void Should_be_able_to_use_different_contexts()
    {
        var service = new DatabaseContextService();

        var context1 = new DatabaseContext("mock-1", "provider-name", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, service);

        Assert.That(service.Current.Key, Is.EqualTo(context1.Key));

        var context2 = new DatabaseContext("mock-2", "provider-name", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, service);

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));

        using (service.Use("mock-1"))
        {
            Assert.That(service.Current.Key, Is.EqualTo(context1.Key));

            Assert.That(service.Current, Is.Not.Null);
            Assert.That(service.Current.Name, Is.EqualTo(service.Current.Name));
        }

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));

        using (service.Activate(context1))
        {
            Assert.That(service.Current.Key, Is.EqualTo(context1.Key));
        }

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));
    }
}