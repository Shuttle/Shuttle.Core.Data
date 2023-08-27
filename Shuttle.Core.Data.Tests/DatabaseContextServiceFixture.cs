using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class DatabaseContextServiceFixture : Fixture
{
    [Test]
    public void Should_be_able_to_use_different_contexts()
    {
        var cache = new DatabaseContextService();

        var context1 = new DatabaseContext("mock-1", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, cache).WithName("mock-1");

        Assert.That(cache.Current.Key, Is.EqualTo(context1.Key));

        var context2 = new DatabaseContext("mock-2", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, cache).WithName("mock-2");

        Assert.That(cache.Current.Key, Is.EqualTo(context2.Key));

        using (cache.Use("mock-1"))
        {
            Assert.That(cache.Current.Key, Is.EqualTo(context1.Key));
        }

        Assert.That(cache.Current.Key, Is.EqualTo(context2.Key));

        using (cache.Use(context1))
        {
            Assert.That(cache.Current.Key, Is.EqualTo(context1.Key));
        }

        Assert.That(cache.Current.Key, Is.EqualTo(context2.Key));
    }

    [Test]
    public async Task Should_be_able_to_use_different_contexts_async()
    {
        var service = new DatabaseContextService();

        var context1 = new DatabaseContext("mock-1", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, service).WithName("mock-1");

        Assert.That(service.Current.Key, Is.EqualTo(context1.Key));

        var context2 = new DatabaseContext("mock-2", new Mock<DbConnection>().Object, new Mock<IDbCommandFactory>().Object, service).WithName("mock-2");

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));

        using (service.Use("mock-1"))
        {
            Assert.That(service.Current.Key, Is.EqualTo(context1.Key));

            await AssertContextFlow(service, "mock-1");
        }

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));

        using (service.Use(context1))
        {
            Assert.That(service.Current.Key, Is.EqualTo(context1.Key));
        }

        Assert.That(service.Current.Key, Is.EqualTo(context2.Key));
    }

    private async Task AssertContextFlow(IDatabaseContextService service, string name)
    {
        Assert.That(service.Current, Is.Not.Null);
        Assert.That(service.Current.Name, Is.EqualTo(name));

        await Task.Run(() =>
        {
            Assert.That(service.Current, Is.Not.Null);
            Assert.That(service.Current.Name, Is.EqualTo(name));
        });
    }
}