using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class QueryMapperFixture : MappingFixture
    {
        [Test]
        public async Task Should_be_able_to_perform_basic_mapping()
        {
            var mapper = GetQueryMapper();

            var queryRow = RawQuery.Create(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var queryRows = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (var databaseContext = await GetDatabaseContext())
            {
                var item = await mapper.MapObject<BasicMapping>(databaseContext, queryRow);
                var items = await mapper.MapObjects<BasicMapping>(databaseContext, queryRows);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = await mapper.MapRow<BasicMapping>(databaseContext, queryRow);
                var mappedRows = await mapper.MapRows<BasicMapping>(databaseContext, queryRows);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
        {
            var mapper = GetQueryMapper();

            var queryRow = RawQuery.Create(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

            var queryRows = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (var databaseContext = await GetDatabaseContext())
            {
                var item = await mapper.MapObject<BasicMapping>(databaseContext, queryRow);
                var items = await mapper.MapObjects<BasicMapping>(databaseContext, queryRows);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(databaseContext, queryRow).Result;
                var mappedRows = await mapper.MapRows<BasicMapping>(databaseContext, queryRows);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_value_mapping()
        {
            var mapper = GetQueryMapper();

            var queryRow = RawQuery.Create(@"
select top 1
    Id
from
    BasicMapping
");

            var queryRows = RawQuery.Create(@"
select
    Id
from
    BasicMapping
");

            using (var databaseContext = await GetDatabaseContext())
            {
                var value = mapper.MapValue<Guid>(databaseContext, queryRow).Result;
                var values = mapper.MapValues<Guid>(databaseContext, queryRows).Result;

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_dynamic_mapping()
        {
            var queryMapper = GetQueryMapper();

            var queryRow = RawQuery.Create(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var queryRows = RawQuery.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (var databaseContext = await GetDatabaseContext())
            {
                var item = queryMapper.MapItem(databaseContext, queryRow).Result;
                var items = queryMapper.MapItems(databaseContext, queryRows).Result;

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());
            }
        }
    }
}