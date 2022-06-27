using System.Data;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    public class DatabaseContextCacheFixture : Fixture
    {
        [Test]
        public void Should_be_able_to_use_different_contexts()
        {
            var cache = new DatabaseContextCache();

            var context1 = new DatabaseContext("mock-1", new Mock<IDbConnection>().Object, new Mock<IDbCommandFactory>().Object, cache).WithName("mock-1");

            Assert.That(cache.Current.Key, Is.EqualTo(context1.Key));

            var context2 = new DatabaseContext("mock-2", new Mock<IDbConnection>().Object, new Mock<IDbCommandFactory>().Object, cache).WithName("mock-2");

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
    }
}