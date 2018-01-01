using System.Configuration;
using System.Data;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class ConnectionStringServiceFixture
    {
        [Test]
        public void Should_be_able_to_approve_given_valid_connection_strings()
        {
            GetConnectionStringService().Approve(new ConnectionStringSettingsCollection
                {
                    new ConnectionStringSettings("Shuttle",
                                                 "Data Source=.\\sqlexpress;Initial Catalog=Shuttle;Integrated Security=SSPI",
                                                 "System.Data.SqlClient")
                });
        }

        [Test]
        public void Should_be_able_to_approve_valid_connection_strings_from_the_configuration_file()
        {
            GetConnectionStringService().Approve();
        }

        private ConnectionStringService GetConnectionStringService()
        {
#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
            return new ConnectionStringService();
#else
            return new ConnectionStringService(new DefaultDbProviderFactories());
#endif
        }

        [Test]
        public void Should_be_able_to_fail_given_invalid_connection_strings()
        {
            Assert.Throws<DataException>(() =>
            {
                GetConnectionStringService().Approve(
                    new ConnectionStringSettingsCollection
                    {
                        new ConnectionStringSettings("not-here",
                            "Data Source=.;Initial Catalog=i-dont-exist;",
                            "System.Data.SqlClient")
                    });
            });
        }
    }
}
