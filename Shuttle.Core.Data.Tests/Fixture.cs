using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public abstract class Fixture
    {
	    private static readonly IServiceCollection Services = new ServiceCollection();
	    protected static IServiceProvider Provider;

	    protected static string DefaultConnectionStringName = "Shuttle";
		protected static string DefaultProviderName = "System.Data.SqlClient";
		protected static string DefaultConnectionString = "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000";

        protected Fixture()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            Services.AddLogging();

            Services.AddDataAccess(configure =>
	            {
		            configure.AddConnection(DefaultConnectionStringName, DefaultProviderName, DefaultConnectionString);
	            }
            );

            Provider = Services.BuildServiceProvider();
        }

        protected IDbConnectionFactory GetDbConnectionFactory()
        {
	        return Provider.GetRequiredService<IDbConnectionFactory>();
        }

        protected IDatabaseContextFactory GetDatabaseContextFactory()
        {
	        return Provider.GetRequiredService<IDatabaseContextFactory>();
        }

        protected IDatabaseContext GetDatabaseContext()
		{
			return GetDatabaseContextFactory().Create(DefaultConnectionStringName);
		}

        protected IDatabaseGateway GetDatabaseGateway()
        {
	        return Provider.GetRequiredService<IDatabaseGateway>();
        }

        protected IQueryMapper GetQueryMapper()
        {
	        return Provider.GetRequiredService<IQueryMapper>();
        }

        protected ILogger<T> GetNullLogger<T>()
        {
	        return new Logger<T>(new NullLoggerFactory());
        }
    }
}
