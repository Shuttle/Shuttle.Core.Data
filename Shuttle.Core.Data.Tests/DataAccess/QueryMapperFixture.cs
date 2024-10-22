using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests.DataAccess;

[TestFixture]
public class QueryMapperFixture : DataAccessFixture
{
    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_async()
    {
        var mapper = QueryMapper;

        var queryRow = new Query(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

        var queryRows = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var item = await mapper.MapObjectAsync<BasicMapping>(databaseContext, queryRow);

            var items = await mapper.MapObjectsAsync<BasicMapping>(databaseContext, queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = await mapper.MapRowAsync<BasicMapping>(databaseContext, queryRow);

            var mappedRows = await mapper.MapRowsAsync<BasicMapping>(databaseContext, queryRows);

            Assert.That(mappedRow, Is.Not.Null);
            Assert.That(mappedRows.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async()
    {
        var mapper = QueryMapper;

        var queryRow = new Query(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

        var queryRows = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var item = await mapper.MapObjectAsync<BasicMapping>(databaseContext, queryRow);
            var items = await mapper.MapObjectsAsync<BasicMapping>(databaseContext, queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = mapper.MapRowAsync<BasicMapping>(databaseContext, queryRow).Result;
            var mappedRows = await mapper.MapRowsAsync<BasicMapping>(databaseContext, queryRows);

            Assert.That(mappedRow, Is.Not.Null);
            Assert.That(mappedRows.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public void Should_be_able_to_perform_value_mapping()
    {
        Should_be_able_to_perform_value_mapping_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_perform_value_mapping_async()
    {
        await Should_be_able_to_perform_value_mapping_async(false);
    }

    private async Task Should_be_able_to_perform_value_mapping_async(bool sync)
    {
        var mapper = QueryMapper;

        var queryRow = new Query(@"
select top 1
    Id
from
    BasicMapping
");

        var queryRows = new Query(@"
select
    Id
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var value =  await mapper.MapValueAsync<Guid>(databaseContext, queryRow);

            var values = await mapper.MapValuesAsync<Guid>(databaseContext, queryRows);

            Assert.That(value, Is.Not.Null);
            Assert.That(values.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_dynamic_mapping_async()
    {
        var queryMapper = QueryMapper;

        var queryRow = new Query(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

        var queryRows = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var item = await queryMapper.MapItemAsync(databaseContext, queryRow);

            var items = await queryMapper.MapItemsAsync(databaseContext, queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));
        }
    }
}