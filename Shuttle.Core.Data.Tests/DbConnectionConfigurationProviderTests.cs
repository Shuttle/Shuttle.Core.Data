using System;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DbConnectionConfigurationProviderTests
	{
		[Test]
		public void Should_be_able_to_get_a_connection_configuration()
		{
			var provider = new DbConnectionConfigurationProvider();

			var configuration = provider.Get(new DataSource("Shuttle", new Mock<IDbDataParameterFactory>().Object));

			Assert.AreEqual("Shuttle", configuration.Name);
			Assert.AreEqual("Data Source=.;Initial Catalog=Shuttle;Integrated Security=SSPI", configuration.ConnectionString);
			Assert.AreEqual("System.Data.SqlClient", configuration.ProviderName);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Should_not_be_able_to_get_an_invalid_connection_configuration()
		{
			new DbConnectionConfigurationProvider().Get(new DataSource("Invalid", new Mock<IDbDataParameterFactory>().Object));
		}
	}
}