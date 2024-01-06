using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class DataRowMapperFixture : MappingFixture
    {
        [Test]
        public async Task Should_be_able_to_perform_basic_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = new RawQuery(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var rowsQuery = new RawQuery(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapObject<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));
                var items = dataRowMapper.MapObjects<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = dataRowMapper.MapRow<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));
                var mappedRows = dataRowMapper.MapRows<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = new RawQuery(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

            var rowsQuery = new RawQuery(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapObject<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));
                var items = dataRowMapper.MapObjects<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = dataRowMapper.MapRow<BasicMapping>(await databaseGateway.GetRowAsync(rowQuery));
                var mappedRows = dataRowMapper.MapRows<BasicMapping>(await databaseGateway.GetRowsAsync(rowsQuery));

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_value_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = new RawQuery(@"
select top 1
    Id
from
    BasicMapping
");

            var rowsQuery = new RawQuery(@"
select
    Id
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var value = dataRowMapper.MapValue<Guid>(await databaseGateway.GetRowAsync(rowQuery));
                var values = dataRowMapper.MapValues<Guid>(await databaseGateway.GetRowsAsync(rowsQuery));

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_dynamic_mapping()
        {
            var id = new Guid("B5E0088E-4873-4244-9B91-1059E0383C3E");

            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

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
            
            var rowQuery = new RawQuery(rowSql).AddParameterValue(Columns.Id, id);
            var rowsQuery = new RawQuery(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapItem(await databaseGateway.GetRowAsync(rowQuery));

                Assert.IsNotNull(item);

                var items = dataRowMapper.MapItems(await databaseGateway.GetRowsAsync(rowsQuery));
                
                Assert.AreEqual(2, items.Count());

                item = dataRowMapper.MapItem(await databaseGateway.GetRowAsync(new RawQuery(rowSql), (object)(new { Id = id })));

                Assert.IsNotNull(item);
                Assert.That(item.Id, Is.EqualTo(id));
                Assert.That(item.Name, Is.EqualTo("Name-2"));
                Assert.That(item.Age, Is.EqualTo(50));
            }
        }
    }
}