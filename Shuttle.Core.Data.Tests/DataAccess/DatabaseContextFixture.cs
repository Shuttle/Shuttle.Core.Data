using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests.DataAccess;

[TestFixture]
public class DatabaseContextFixture : DataAccessFixture
{
    [Test]
    public async Task Should_be_able_to_get_data_table_async()
    {
        DataTable table;

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            table = await databaseContext.GetDataTableAsync(new Query("select * from BasicMapping"));
        }

        Assert.That(table, Is.Not.Null);
        Assert.That(table.Rows.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_execute_a_query_async()
    {
        int count;

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            count = await databaseContext.ExecuteAsync(new Query("update BasicMapping set Age = Age"));
        }

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_get_reader_async()
    {
        var count = 0;

        await using (var databaseContext = DatabaseContextFactory.Create())
        await using (var reader = await databaseContext.GetReaderAsync(new Query("select * from BasicMapping")))
        {
            while (await reader.ReadAsync())
            {
                count++;
            }
        }

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_get_rows_async()
    {
        IEnumerable<DataRow> rows;

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            rows = await databaseContext.GetRowsAsync(new Query("select * from BasicMapping"));
        }

        Assert.That(rows, Is.Not.Null);
        Assert.That(rows.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_get_row_async()
    {
        DataRow rows;

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            rows = await databaseContext.GetRowAsync(new Query("select top 1 * from BasicMapping"));
        }

        Assert.That(rows, Is.Not.Null);
    }

    [Test]
    public async Task Should_be_able_to_get_scalar_async()
    {
        int number;

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            number = await databaseContext.GetScalarAsync<int>(new Query("select 175"));
        }

        Assert.That(number, Is.EqualTo(175));
    }
}
