using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DatabaseContextFixture : Fixture
{
    [Test]
    public void Should_not_be_able_to_create_an_invalid_connection()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            using (new DatabaseContext("context", "Microsoft.Data.SqlClient", new SqlConnection("```"), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
            {
            }
        });
    }

    [Test]
    public void Should_not_be_able_to_create_a_non_existent_connection()
    {
        Assert.ThrowsAsync<SqlException>(async () =>
        {
            await using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", new SqlConnection("data source=.;initial catalog=idontexist;integrated security=sspi"), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
            {
                _ = await databaseContext.CreateCommandAsync(new Query("select 1"));
            }
        });
    }

    [Test]
    public async Task Should_be_able_to_begin_and_commit_a_transaction_async()
    {
        await using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            await databaseContext.BeginTransactionAsync();
            await databaseContext.CommitTransactionAsync();
        }
    }

    [Test]
    public async Task Should_be_able_to_begin_and_rollback_a_transaction_async()
    {
        await using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            await databaseContext.BeginTransactionAsync();
        }
    }

    [Test]
    public async Task Should_be_able_to_call_commit_without_a_transaction_async()
    {
        await using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            await databaseContext.CommitTransactionAsync();
        }
    }

    [Test]
    public void Should_be_able_to_call_dispose_more_than_once()
    {
        using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            databaseContext.Dispose();
            databaseContext.Dispose();
        }
    }

    [Test]
    public async Task Should_be_able_to_create_a_command()
    {
        Mock<IDbCommandFactory> dbCommandFactory = new();
        var dbConnection = DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString);
        Mock<IQuery> query = new();
        Mock<DbCommand> dbCommand = new();

        dbCommandFactory.Setup(m => m.Create(dbConnection, query.Object)).Returns(dbCommand.Object);

        await using (DatabaseContext databaseContext = new("context", "Microsoft.Data.SqlClient", dbConnection, dbCommandFactory.Object, DatabaseContextService))
        {
            _ = await databaseContext.CreateCommandAsync(query.Object);
        }

        dbCommandFactory.VerifyAll();
    }
}