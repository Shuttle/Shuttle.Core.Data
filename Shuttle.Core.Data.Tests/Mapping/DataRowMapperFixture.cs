using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DataRowMapperFixture : MappingFixture
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
        var rowQuery = new Query(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

        var rowsQuery = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (DatabaseContextFactory.Create())
        {
            var item = sync
                ? DataRowMapper.MapObject<BasicMapping>(DatabaseGateway.GetRow(rowQuery))
                : DataRowMapper.MapObject<BasicMapping>(await DatabaseGateway.GetRowAsync(rowQuery));

            var items = sync
                ? DataRowMapper.MapObjects<BasicMapping>(DatabaseGateway.GetRows(rowsQuery))
                : DataRowMapper.MapObjects<BasicMapping>(await DatabaseGateway.GetRowsAsync(rowsQuery));

            Assert.IsNotNull(item);
            Assert.AreEqual(2, items.Count());

            var mappedRow = sync
                ? DataRowMapper.MapRow<BasicMapping>(DatabaseGateway.GetRow(rowQuery))
                : DataRowMapper.MapRow<BasicMapping>(await DatabaseGateway.GetRowAsync(rowQuery));
            var mappedRows = sync
                ? DataRowMapper.MapRows<BasicMapping>(DatabaseGateway.GetRows(rowsQuery))
                : DataRowMapper.MapRows<BasicMapping>(await DatabaseGateway.GetRowsAsync(rowsQuery));

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
        var databaseGateway = DatabaseGateway;
        var dataRowMapper = DataRowMapper;

        var rowQuery = new Query(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

        var rowsQuery = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (DatabaseContextFactory.Create())
        {
            var item = sync
                ? dataRowMapper.MapObject<BasicMapping>(databaseGateway.GetRow(rowQuery))
                : dataRowMapper.MapObject<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));

            var items = sync
                ? dataRowMapper.MapObjects<BasicMapping>(databaseGateway.GetRows(rowsQuery))
                : dataRowMapper.MapObjects<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

            Assert.IsNotNull(item);
            Assert.AreEqual(2, items.Count());

            var mappedRow = sync
                ? dataRowMapper.MapRow<BasicMapping>(databaseGateway.GetRow(rowQuery))
                : dataRowMapper.MapRow<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));

            var mappedRows = sync
                ? dataRowMapper.MapRows<BasicMapping>(databaseGateway.GetRows(rowsQuery))
                : dataRowMapper.MapRows<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

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
        var databaseGateway = DatabaseGateway;
        var dataRowMapper = DataRowMapper;

        var rowQuery = new Query(@"
select top 1
    Id
from
    BasicMapping
");

        var rowsQuery = new Query(@"
select
    Id
from
    BasicMapping
");

        await using (DatabaseContextFactory.Create())
        {
            var value = sync
                ? dataRowMapper.MapValue<Guid>(databaseGateway.GetRow(rowQuery))
                : dataRowMapper.MapValue<Guid>(await databaseGateway.GetRowAsync(rowQuery));

            var values = sync
                ? dataRowMapper.MapValues<Guid>(databaseGateway.GetRows(rowsQuery))
                : dataRowMapper.MapValues<Guid>(await databaseGateway.GetRowsAsync(rowsQuery));

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


    private async Task Should_be_able_to_perform_dynamic_mapping_async(bool sync)
    {
        var id = new Guid("B5E0088E-4873-4244-9B91-1059E0383C3E");

        var databaseGateway = DatabaseGateway;
        var dataRowMapper = DataRowMapper;

        var rowSql = @"
select 
    Id,
    Name,
    Age
from
    BasicMapping
where
    Id = @Id
";

        var rowQuery = new Query(rowSql).AddParameter(Columns.Id, id);
        var rowsQuery = new Query(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (DatabaseContextFactory.Create())
        {
            var item = sync
                ? dataRowMapper.MapItem(databaseGateway.GetRow(rowQuery))
                : dataRowMapper.MapItem(await databaseGateway.GetRowAsync(rowQuery));

            Assert.IsNotNull(item);

            var items = sync
                ? dataRowMapper.MapItems(databaseGateway.GetRows(rowsQuery))
                : dataRowMapper.MapItems(await databaseGateway.GetRowsAsync(rowsQuery));

            Assert.AreEqual(2, items.Count());

            item = sync
                ? dataRowMapper.MapItem(databaseGateway.GetRow(new Query(rowSql).AddParameters(new { Id = id })))
                : dataRowMapper.MapItem(await databaseGateway.GetRowAsync(new Query(rowSql).AddParameters(new { Id = id })));

            Assert.IsNotNull(item);
            Assert.That(item.Id, Is.EqualTo(id));
            Assert.That(item.Name, Is.EqualTo("Name-2"));
            Assert.That(item.Age, Is.EqualTo(50));
        }
    }
}