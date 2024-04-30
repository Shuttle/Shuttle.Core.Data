using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class QueryMapperFixture : MappingFixture
{
    [Test]
    public void Should_be_able_to_perform_basic_mapping()
    {
        Should_be_able_to_perform_basic_mapping_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_async()
    {
        await Should_be_able_to_perform_basic_mapping_async(false);
    }

    private async Task Should_be_able_to_perform_basic_mapping_async(bool sync)
    {
        var mapper = GetQueryMapper();

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

        using (GetDatabaseContext())
        {
            var item = sync
                ? mapper.MapObject<BasicMapping>(queryRow)
                : await mapper.MapObjectAsync<BasicMapping>(queryRow);

            var items = sync
                ? mapper.MapObjects<BasicMapping>(queryRows)
                : await mapper.MapObjectsAsync<BasicMapping>(queryRows);

            Assert.IsNotNull(item);
            Assert.AreEqual(2, items.Count());

            var mappedRow = sync
                ? mapper.MapRow<BasicMapping>(queryRow)
                : await mapper.MapRowAsync<BasicMapping>(queryRow);

            var mappedRows = sync
                ? mapper.MapRows<BasicMapping>(queryRows)
                : await mapper.MapRowsAsync<BasicMapping>(queryRows);

            Assert.IsNotNull(mappedRow);
            Assert.AreEqual(2, mappedRows.Count());
        }
    }

    [Test]
    public void Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
    {
        Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async()
    {
        await Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async(false);
    }

    private async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async(bool sync)
    {
        var mapper = GetQueryMapper();

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

        using (GetDatabaseContext())
        {
            var item = await mapper.MapObjectAsync<BasicMapping>(queryRow);
            var items = await mapper.MapObjectsAsync<BasicMapping>(queryRows);

            Assert.IsNotNull(item);
            Assert.AreEqual(2, items.Count());

            var mappedRow = mapper.MapRowAsync<BasicMapping>(queryRow).Result;
            var mappedRows = await mapper.MapRowsAsync<BasicMapping>(queryRows);

            Assert.IsNotNull(mappedRow);
            Assert.AreEqual(2, mappedRows.Count());
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
        var mapper = GetQueryMapper();

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

        using (GetDatabaseContext())
        {
            var value = sync
                ? mapper.MapValue<Guid>(queryRow)
                : await mapper.MapValueAsync<Guid>(queryRow);

            var values = sync
                ? mapper.MapValues<Guid>(queryRows)
                : await mapper.MapValuesAsync<Guid>(queryRows);

            Assert.IsNotNull(value);
            Assert.AreEqual(2, values.Count());
        }
    }

    [Test]
    public void Should_be_able_to_perform_dynamic_mapping()
    {
        Should_be_able_to_perform_dynamic_mapping_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_perform_dynamic_mapping_async()
    {
        await Should_be_able_to_perform_dynamic_mapping_async(false);
    }

    public async Task Should_be_able_to_perform_dynamic_mapping_async(bool sync)
    {
        var queryMapper = GetQueryMapper();

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

        using (GetDatabaseContext())
        {
            var item = sync
                ? queryMapper.MapItem(queryRow)
                : await queryMapper.MapItemAsync(queryRow);

            var items = sync
                ? queryMapper.MapItems(queryRows)
                : await queryMapper.MapItemsAsync(queryRows);

            Assert.IsNotNull(item);
            Assert.AreEqual(2, items.Count());
        }
    }
}