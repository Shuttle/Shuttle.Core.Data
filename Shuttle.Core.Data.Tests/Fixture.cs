using System;
using System.Data.Common;
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
		            builder.Options.DatabaseContextFactory.DefaultConnectionStringName = DefaultConnectionStringName;
	            }
            );

            Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            Provider = Services.BuildServiceProvider();

            DbConnectionFactory = Provider.GetRequiredService<IDbConnectionFactory>();
            DatabaseContextFactory = Provider.GetRequiredService<IDatabaseContextFactory>();
            DatabaseGateway = Provider.GetRequiredService<IDatabaseGateway>();
            QueryMapper = Provider.GetRequiredService<IQueryMapper>();
            DataRowMapper = Provider.GetRequiredService<IDataRowMapper>();
            DatabaseContextService = Provider.GetRequiredService<IDatabaseContextService>();
        }

        protected IDbConnectionFactory DbConnectionFactory { get; }
        protected IDatabaseContextFactory DatabaseContextFactory { get; }
        protected IDatabaseGateway DatabaseGateway { get; }
        protected IQueryMapper QueryMapper { get; }
        protected IDataRowMapper DataRowMapper { get; }
        protected IDatabaseContextService DatabaseContextService { get; }
    }
}
