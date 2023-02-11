using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public abstract class Fixture
    {
	    private static readonly IServiceCollection Services = new ServiceCollection();
	    protected static IServiceProvider Provider;

	    protected static string DefaultConnectionStringName = "Shuttle";
		protected static string DefaultProviderName = "Microsoft.Data.SqlClient";
		protected static string DefaultConnectionString = "Server=.;Database=Shuttle;User ID=sa;Password=Pass!000;TrustServerCertificate=true";

        protected Fixture()
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

            Services.AddDataAccess(builder =>
	            {
		            builder.AddConnectionString(DefaultConnectionStringName, DefaultProviderName, DefaultConnectionString);
	            }
            );

            Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

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

        protected async Task<IDatabaseContext> GetDatabaseContext()
		{
			return await GetDatabaseContextFactory().Create(DefaultConnectionStringName).ConfigureAwait(false);
		}

        protected IDatabaseGateway GetDatabaseGateway()
        {
	        return Provider.GetRequiredService<IDatabaseGateway>();
        }

        protected IQueryMapper GetQueryMapper()
        {
	        return Provider.GetRequiredService<IQueryMapper>();
        }

        protected IDataRowMapper GetDataRowMapper()
        {
	        return Provider.GetRequiredService<IDataRowMapper>();
        }
    }
}
