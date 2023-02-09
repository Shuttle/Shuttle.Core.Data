using System;
using System.Linq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class DataRowMapperFixture : MappingFixture
    {
        [Test]
        public void Should_be_able_to_perform_basic_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = RawQuery.Create(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var rowsQuery = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapObject<BasicMapping>(databaseGateway.GetRow(rowQuery).Result);
                var items = dataRowMapper.MapObjects<BasicMapping>(databaseGateway.GetRows(rowsQuery).Result);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = dataRowMapper.MapRow<BasicMapping>(databaseGateway.GetRow(rowQuery).Result);
                var mappedRows = dataRowMapper.MapRows<BasicMapping>(databaseGateway.GetRows(rowsQuery).Result);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = RawQuery.Create(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

            var rowsQuery = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapObject<BasicMapping>(databaseGateway.GetRow(rowQuery).Result);
                var items = dataRowMapper.MapObjects<BasicMapping>(databaseGateway.GetRows(rowsQuery).Result);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = dataRowMapper.MapRow<BasicMapping>(databaseGateway.GetRow(rowQuery).Result);
                var mappedRows = dataRowMapper.MapRows<BasicMapping>(databaseGateway.GetRows(rowsQuery).Result);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_value_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
            var dataRowMapper = GetDataRowMapper();

            var rowQuery = RawQuery.Create(@"
select top 1
    Id
from
    BasicMapping
");

            var rowsQuery = RawQuery.Create(@"
select
    Id
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var value = dataRowMapper.MapValue<Guid>(databaseGateway.GetRow(rowQuery).Result);
                var values = dataRowMapper.MapValues<Guid>(databaseGateway.GetRows(rowsQuery).Result);

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_dynamic_mapping()
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
            
            var rowQuery = RawQuery.Create(rowSql).AddParameterValue(Columns.Id, id);
            var rowsQuery = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = dataRowMapper.MapItem(databaseGateway.GetRow(rowQuery).Result);

                Assert.IsNotNull(item);

                var items = dataRowMapper.MapItems(databaseGateway.GetRows(rowsQuery).Result);
                
                Assert.AreEqual(2, items.Count());

                item = dataRowMapper.MapItem(databaseGateway.GetRow(RawQuery.Create(rowSql, new { Id = id })).Result);

                Assert.IsNotNull(item);
                Assert.That(item.Id, Is.EqualTo(id));
                Assert.That(item.Name, Is.EqualTo("Name-2"));
                Assert.That(item.Age, Is.EqualTo(50));
            }
        }
    }
}