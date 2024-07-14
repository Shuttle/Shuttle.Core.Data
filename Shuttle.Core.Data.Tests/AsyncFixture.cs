using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class AsyncFixture : MappingFixture
{
    private readonly Query _rowsQuery = new(@"
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

        var databaseContextService = Provider.GetRequiredService<IDatabaseContextService>();

        var databaseContext = databaseContextService.Find(context => context.Name.Equals(DefaultConnectionStringName));

        Assert.That(databaseContext, Is.Null);

        for (var i = 0; i < 10; i++)
        {
            using (ExecutionContext.SuppressFlow())
            {
                threads.Add(new Thread(() =>
                {
                    databaseContextService.SetAmbientScope();

                    using (GetDatabaseContext())
                    {
                        GetDatabaseGateway().GetRowsAsync(_rowsQuery);
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
    public void Should_be_able_to_use_the_different_database_context_for_separate_tasks_async()
    {
        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                using (GetDatabaseContext())
                {
                    GetDatabaseGateway().GetRowsAsync(_rowsQuery);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    [Test]
    public void Should_be_able_to_use_the_same_database_context_across_tasks()
    {
        var tasks = new List<Task>();

        using (GetDatabaseContext())
        {
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(GetDatabaseGateway().GetRowsAsync(_rowsQuery));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    [Test]
    public async Task Should_be_able_to_use_the_same_database_context_across_synchronized_tasks_async()
    {
        using (GetDatabaseContext())
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

        _ = await GetDatabaseGateway().GetRowsAsync(_rowsQuery);
    }
}