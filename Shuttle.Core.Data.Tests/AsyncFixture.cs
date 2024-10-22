using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;
using Shuttle.Core.Data.Tests.DataAccess;

namespace Shuttle.Core.Data.Tests;

public class AsyncFixture : DataAccessFixture
{
    private readonly Query _rowsQuery = new(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

    private async Task GetRowsAsync(int depth)
    {
        if (depth < 5)
        {
            await GetRowsAsync(depth + 1);
        }

        _ = await DatabaseContextFactory.Create().GetRowsAsync(_rowsQuery);
    }

    [Test]
    public async Task Should_be_able_to_create_nested_database_context_scopes_async()
    {
        await using (DatabaseContextFactory.Create())
        {
            await DatabaseContextFactory.Create().ExecuteAsync(new Query("DROP TABLE IF EXISTS dbo.Nested"));
            await DatabaseContextFactory.Create().ExecuteAsync(new Query("CREATE TABLE dbo.Nested (Id int)"));
        }

        await using (var databaseContextOuter = DatabaseContextFactory.Create())
        {
            var count = await databaseContextOuter.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.Zero);

            await databaseContextOuter.ExecuteAsync(new Query("INSERT INTO dbo.Nested (Id) VALUES (1)"));

            count = await databaseContextOuter.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));

            using (new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            await using (var databaseContextInner = DatabaseContextFactory.Create())
            {
                await databaseContextInner.ExecuteAsync(new Query("INSERT INTO dbo.Nested (Id) VALUES (2)"));

                count = await databaseContextInner.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

                Assert.That(count, Is.EqualTo(2));
            }

            count = await databaseContextOuter.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));
        }

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var count = await databaseContext.GetScalarAsync<int>(new Query("select count(*) from dbo.Nested"));

            Assert.That(count, Is.EqualTo(1));

            await databaseContext.ExecuteAsync(new Query("DROP TABLE IF EXISTS dbo.Nested"));
        }
    }

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_tasks_async()
    {
        List<Task> tasks = new();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await using (var databaseContext = DatabaseContextFactory.Create())
                {
                    _ = await databaseContext.GetRowsAsync(_rowsQuery);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_threads_async()
    {
        List<Thread> threads = new();

        for (var i = 0; i < 10; i++)
        {
            using (ExecutionContext.SuppressFlow())
            {
                threads.Add(new(async () =>
                {
                    await using (var databaseContext = DatabaseContextFactory.Create())
                    {
                        await databaseContext.GetRowsAsync(_rowsQuery);
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
    public async Task Should_be_able_to_use_the_same_database_context_across_synchronized_tasks_async()
    {
        await using (DatabaseContextFactory.Create())
        {
            await GetRowsAsync(0);
        }
    }
}