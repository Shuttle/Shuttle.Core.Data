using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class ConnectionConfigurationProviderFixture
    {
        [Test]
        public void Should_be_able_to_get_configuration()
        {
            var provider = new ConnectionConfigurationProvider();

            Assert.That(provider.Get("not-defined"), Is.Null);

            var configuration = provider.Get("Shuttle");

            Assert.That(configuration.Name, Is.EqualTo("Shuttle"));
            Assert.That(configuration.ProviderName, Is.EqualTo("System.Data.SqlClient"));
            Assert.That(configuration.ConnectionString, Is.EqualTo("Data Source=.\\sqlexpress;Initial Catalog=Shuttle;Integrated Security=SSPI"));
        }
    }
}