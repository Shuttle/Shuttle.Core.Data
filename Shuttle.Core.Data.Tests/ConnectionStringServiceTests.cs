using System;
using System.Configuration;
using System.Data;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class ConnectionStringServiceTests
    {
        [Test]
        public void Should_be_able_to_approve_given_valid_connection_strings()
        {
            new ConnectionStringService().Approve(new ConnectionStringSettingsCollection
                {
                    new ConnectionStringSettings("Shuttle",
                                                 "Data Source=.\\sqlexpress;Initial Catalog=Shuttle;Integrated Security=SSPI",
                                                 "System.Data.SqlClient")
                });
        }

        [Test]
        public void Should_be_able_to_approve_valid_connection_strings_from_the_configuration_file()
        {
            new ConnectionStringService().Approve();
        }

        [Test]
        public void Should_be_able_to_fail_given_invalid_connection_strings()
        {
            Assert.Throws<DataException>(() =>
                new ConnectionStringService().Approve(new ConnectionStringSettingsCollection
                {
                        new ConnectionStringSettings("not-here",
                            "Data Source=.;Initial Catalog=i-dont-exist;",
                            "System.Data.SqlClient")
                }));
        }
    }
}
