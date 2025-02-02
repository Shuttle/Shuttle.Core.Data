using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests.DataAccess;

[TestFixture]
public class DataRowMapperFixture : DataAccessFixture
{
    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_async()
    {
        Query rowQuery = new(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

        Query rowsQuery = new(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var row = await databaseContext.GetRowAsync(rowQuery);

            Assert.That(row, Is.Not.Null);

            var item = DataRowMapper.MapObject<BasicMapping>(row);

            var items = DataRowMapper.MapObjects<BasicMapping>(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = DataRowMapper.MapRow<BasicMapping>(row);

            var mappedRows = DataRowMapper.MapRows<BasicMapping>(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(mappedRow, Is.Not.Null);
            Assert.That(mappedRows.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing_async()
    {
        var dataRowMapper = DataRowMapper;

        Query rowQuery = new(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

        Query rowsQuery = new(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var row = await databaseContext.GetRowAsync(rowQuery);

            Assert.That(row, Is.Not.Null);

            var item = dataRowMapper.MapObject<BasicMapping>(row);

            var items = dataRowMapper.MapObjects<BasicMapping>(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(item, Is.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(2));

            var mappedRow = dataRowMapper.MapRow<BasicMapping>(row);

            var mappedRows = dataRowMapper.MapRows<BasicMapping>(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(mappedRow, Is.Not.Null);
            Assert.That(mappedRows.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_value_mapping_async()
    {
        var dataRowMapper = DataRowMapper;

        Query rowQuery = new(@"
select top 1
    Id
from
    BasicMapping
");

        Query rowsQuery = new(@"
select
    Id
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var row = await databaseContext.GetRowAsync(rowQuery);

            Assert.That(row, Is.Not.Null);

            var value = dataRowMapper.MapValue<Guid>(row);

            var values = dataRowMapper.MapValues<Guid>(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(value, Is.Not.Null);
            Assert.That(values.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task Should_be_able_to_perform_dynamic_mapping_async()
    {
        Guid id = new("B5E0088E-4873-4244-9B91-1059E0383C3E");

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
        Query rowsQuery = new(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

        await using (var databaseContext = DatabaseContextFactory.Create())
        {
            var row = await databaseContext.GetRowAsync(rowQuery);

            Assert.That(row, Is.Not.Null);

            var item = dataRowMapper.MapItem(row);

            Assert.That(item, Is.Not.Null);

            var items = dataRowMapper.MapItems(await databaseContext.GetRowsAsync(rowsQuery));

            Assert.That(items.Count(), Is.EqualTo(2));

            row = await databaseContext.GetRowAsync(new Query(rowSql).AddParameters(new { Id = id }));

            Assert.That(row, Is.Not.Null);

            item = dataRowMapper.MapItem(row);

            Assert.That(item, Is.Not.Null);
            Assert.That(item.Id, Is.EqualTo(id));
            Assert.That(item.Name, Is.EqualTo("Name-2"));
            Assert.That(item.Age, Is.EqualTo(50));
        }
    }
}