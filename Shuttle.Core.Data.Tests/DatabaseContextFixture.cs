﻿using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Threading;

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
        Assert.Throws<SqlException>(() =>
        {
            using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", new SqlConnection("data source=.;initial catalog=idontexist;integrated security=sspi"), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
            {
                databaseContext.CreateCommand(new Query("select 1"));
            }
        });
    }

    [Test]
    public void Should_be_able_to_begin_and_commit_a_transaction()
    {
        Should_be_able_to_begin_and_commit_a_transaction_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_begin_and_commit_a_transaction_async()
    {
        await Should_be_able_to_begin_and_commit_a_transaction_async(false);
    }

    private async Task Should_be_able_to_begin_and_commit_a_transaction_async(bool sync)
    {
        await using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            if (sync)
            {
                databaseContext.BeginTransaction();
                databaseContext.CommitTransaction();
            }
            else
            {
                await databaseContext.BeginTransactionAsync();
                await databaseContext.CommitTransactionAsync();
            }
        }
    }

    [Test]
    public void Should_be_able_to_begin_and_rollback_a_transaction()
    {
        Should_be_able_to_begin_and_rollback_a_transaction_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_begin_and_rollback_a_transaction_async()
    {
        await Should_be_able_to_begin_and_rollback_a_transaction_async(false);
    }

    private async Task Should_be_able_to_begin_and_rollback_a_transaction_async(bool sync)
    {
        await using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
        {
            if (sync)
            {
                databaseContext.BeginTransaction();
            }
            else
            {
                await databaseContext.BeginTransactionAsync();
            }
        }
    }

    [Test]
    public void Should_be_able_to_call_commit_without_a_transaction()
    {
        Should_be_able_to_call_commit_without_a_transaction_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_call_commit_without_a_transaction_async()
    {
        await Should_be_able_to_call_commit_without_a_transaction_async(false);
    }

    private async Task Should_be_able_to_call_commit_without_a_transaction_async(bool sync)
    {
        await using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            if (sync)
            {
                databaseContext.CommitTransaction();
            }
            else
            {
                await databaseContext.CommitTransactionAsync();
            }
        }
    }

    [Test]
    public void Should_be_able_to_call_dispose_more_than_once()
    {
        using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString), new Mock<IDbCommandFactory>().Object, DatabaseContextService))
        {
            databaseContext.Dispose();
            databaseContext.Dispose();
        }
    }

    [Test]
    public void Should_be_able_to_create_a_command()
    {
        var dbCommandFactory = new Mock<IDbCommandFactory>();
        var dbConnection = DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString);
        var query = new Mock<IQuery>();
        var dbCommand = new Mock<DbCommand>();

        dbCommandFactory.Setup(m => m.Create(dbConnection, query.Object)).Returns(dbCommand.Object);

        using (var databaseContext = new DatabaseContext("context", "Microsoft.Data.SqlClient", dbConnection, dbCommandFactory.Object, DatabaseContextService))
        {
            databaseContext.CreateCommand(query.Object);
        }

        dbCommandFactory.VerifyAll();
    }
}