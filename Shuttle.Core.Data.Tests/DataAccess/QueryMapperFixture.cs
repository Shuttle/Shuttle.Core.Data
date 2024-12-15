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

        await using (DatabaseContextFactory.Create())
        {
            var item = await mapper.MapObjectAsync<BasicMapping>(queryRow);

            var items = await mapper.MapObjectsAsync<BasicMapping>(queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = await mapper.MapRowAsync<BasicMapping>(queryRow);

            var mappedRows = await mapper.MapRowsAsync<BasicMapping>(queryRows);

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

        await using (DatabaseContextFactory.Create())
        {
            var item = await mapper.MapObjectAsync<BasicMapping>(queryRow);
            var items = await mapper.MapObjectsAsync<BasicMapping>(queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = mapper.MapRowAsync<BasicMapping>(queryRow).Result;
            var mappedRows = await mapper.MapRowsAsync<BasicMapping>(queryRows);

            Assert.That(mappedRow, Is.Not.Null);
            Assert.That(mappedRows.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_value_mapping_async()
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

        await using (DatabaseContextFactory.Create())
        {
            var value =  await mapper.MapValueAsync<Guid>(queryRow);

            var values = await mapper.MapValuesAsync<Guid>(queryRows);

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

        await using (DatabaseContextFactory.Create())
        {
            var item = await queryMapper.MapItemAsync(queryRow);

            var items = await queryMapper.MapItemsAsync(queryRows);

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));
        }
    }
}