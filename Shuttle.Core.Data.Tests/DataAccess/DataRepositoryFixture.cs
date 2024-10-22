using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests.DataAccess;

[TestFixture]
public class DataRepositoryFixture : DataAccessFixture
{
    [Test]
    public async Task Should_be_able_to_fetch_all_items_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.FetchItemsAsync(DatabaseContextFactory.Create(), new Query("select * from BasicMapping"))).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_fetch_a_single_item_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.FetchItemAsync(DatabaseContextFactory.Create(), new Query("select top 1 * from BasicMapping")));

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.FetchItemAsync(DatabaseContextFactory.Create(), new Query("select top 1 * from BasicMapping where Name = 'not-found'")));

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Should_be_able_to_call_contains_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.ContainsAsync(DatabaseContextFactory.Create(), new Query("select 1")));

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Should_be_able_to_fetch_mapped_rows_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.FetchMappedRowsAsync(DatabaseContextFactory.Create(), new Query("select * from BasicMapping"))).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Should_be_able_to_fetch_a_single_row_async()
    {
        var repository = new DataRepository<BasicMapping>(new BasicDataRowMapper());

        var result = (await repository.FetchMappedRowAsync(DatabaseContextFactory.Create(), new Query("select top 1 * from BasicMapping")));

        Assert.That(result, Is.Not.Null);
    }
}

public class BasicDataRowMapper : IDataRowMapper<BasicMapping>
{
    public MappedRow<BasicMapping> Map(DataRow row)
    {
        return new(row, new()
        {
            Id = Columns.Id.Value(row),
            Age = Columns.Age.Value(row),
            Name = Columns.Name.Value(row)
        });
    }
}