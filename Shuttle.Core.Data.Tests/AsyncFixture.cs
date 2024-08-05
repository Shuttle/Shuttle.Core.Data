using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class AsyncFixture : MappingFixture
{
    private readonly Query _rowsQuery = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_threads_async()
    {
        var threads = new List<Thread>();

        var databaseContext = DatabaseContextService.Find(context => context.Name.Equals(DefaultConnectionStringName));

        Assert.That(databaseContext, Is.Null);

        for (var i = 0; i < 10; i++)
        {
            using (ExecutionContext.SuppressFlow())
            {
                threads.Add(new Thread(() =>
                {
                    using (new DatabaseContextScope())
                    using (DatabaseContextFactory.Create())
                    {
                        DatabaseGateway.GetRowsAsync(_rowsQuery);
                    }
                }));
            }
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    [Test]
    public void Should_not_be_able_to_create_duplicate_database_contexts_async()
    {
        using (new DatabaseContextScope())
        using (DatabaseContextFactory.Create())
        {
            Assert.That(()=> DatabaseContextFactory.Create(), Throws.InvalidOperationException);
        }
    }

    [Test]
    public async Task Should_be_able_to_create_nested_database_context_scopes_async()
    {
        await using (DatabaseContextFactory.Create())
        {
            await DatabaseGateway.ExecuteAsync(new Query("DROP TABLE IF EXISTS dbo.Nested"));
            await DatabaseGateway.ExecuteAsync(new Query("CREATE TABLE dbo.Nested (Id int)"));
        }

        using (new DatabaseContextScope())
        await using (DatabaseContextFactory.Create())
        {
            var count = await DatabaseGateway.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.Zero);

            await DatabaseGateway.ExecuteAsync(new Query("INSERT INTO dbo.Nested (Id) VALUES (1)"));

            count = await DatabaseGateway.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));

            using (new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            using (new DatabaseContextScope())
            await using (DatabaseContextFactory.Create())
            {
                await DatabaseGateway.ExecuteAsync(new Query("INSERT INTO dbo.Nested (Id) VALUES (2)"));

                count = await DatabaseGateway.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

                Assert.That(count, Is.EqualTo(2));
            }

            count = await DatabaseGateway.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));
        }

        await using (DatabaseContextFactory.Create())
        {
            var count = await DatabaseGateway.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));

            await DatabaseGateway.ExecuteAsync(new Query("DROP TABLE IF EXISTS dbo.Nested"));
        }
    }

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_tasks_async()
    {
        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                using (new DatabaseContextScope())
                using (DatabaseContextFactory.Create())
                {
                    DatabaseGateway.GetRowsAsync(_rowsQuery);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    [Test]
    public async Task Should_be_able_to_use_the_same_database_context_across_synchronized_tasks_async()
    {
        await using (DatabaseContextFactory.Create())
        {
            await GetRowsAsync(0);
        }
    }

    private async Task GetRowsAsync(int depth)
    {
        if (depth < 5)
        {
            await GetRowsAsync(depth + 1);
        }

        _ = await DatabaseGateway.GetRowsAsync(_rowsQuery);
    }
}