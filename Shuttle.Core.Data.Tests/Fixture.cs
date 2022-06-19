using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public abstract class Fixture
    {
	    protected static string DefaultConnectionStringName = "Shuttle";
		protected static string DefaultProviderName = "System.Data.SqlClient";
		protected static string DefaultConnectionString = "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000";

        protected Fixture()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        }

        protected DbConnectionFactory GetDbConnectionFactory()
        {
            return new DbConnectionFactory();
        }

	    protected IDatabaseContext GetDatabaseContext()
		{
			return new DatabaseContextFactory(GetConnectionConfigurationProvider(), GetDbConnectionFactory(), new DbCommandFactory(Options.Create(new DbCommandFactorySettings())), new ThreadStaticDatabaseContextCache()).Create(DefaultConnectionStringName);
		}

        protected IConnectionConfigurationProvider GetConnectionConfigurationProvider()
        {
            var provider = new Mock<IConnectionConfigurationProvider>();

            provider.Setup(m => m.Get("Shuttle")).Returns(new ConnectionConfiguration(
                "Shuttle",
                "System.Data.SqlClient",
                "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000"));

            return provider.Object;
        }

        protected IDatabaseContext GetDatabaseContext(Mock<IDbCommand> command)
	    {
		    var commandFactory = new Mock<IDbCommandFactory>();

		    commandFactory.Setup(m => m.CreateCommandUsing(It.IsAny<IDbConnection>(), It.IsAny<IQuery>())).Returns(command.Object);

			return new DatabaseContextFactory(GetConnectionConfigurationProvider(), GetDbConnectionFactory(), commandFactory.Object, new ThreadStaticDatabaseContextCache()).Create(DefaultConnectionStringName);
	    }
    }
}
