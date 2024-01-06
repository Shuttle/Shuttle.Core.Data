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
                var item = await mapper.MapObjectAsync<BasicMapping>(queryRow);
                var items = await mapper.MapObjectsAsync<BasicMapping>(queryRows);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = await mapper.MapRowAsync<BasicMapping>(queryRow);
                var mappedRows = await mapper.MapRowsAsync<BasicMapping>(queryRows);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public async Task Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
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
                var value = mapper.MapValueAsync<Guid>(queryRow).Result;
                var values = mapper.MapValuesAsync<Guid>(queryRows).Result;

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_dynamic_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
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
                var item = queryMapper.MapItemAsync(queryRow).Result;
                var items = queryMapper.MapItemsAsync(queryRows).Result;

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());
            }
        }
    }
}