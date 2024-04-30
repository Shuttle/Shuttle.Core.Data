using System.Collections.Generic;
using System.Threading.Tasks;
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
    public void Should_be_able_to_use_the_same_database_context_across_tasks_async()
    {
        var tasks = new List<Task>();

        using (GetDatabaseContext())
        {
            for (int i = 0; i < 10; i++)
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