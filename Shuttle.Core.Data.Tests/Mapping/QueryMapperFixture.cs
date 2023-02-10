using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class QueryMapperFixture : MappingFixture
    {
        [Test]
        public void Should_be_able_to_perform_basic_mapping()
        {
            var mapper = GetQueryMapper();

            var queryRow = Query.Create(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var queryRows = Query.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = mapper.MapObject<BasicMapping>(queryRow).Result;
                var items = mapper.MapObjects<BasicMapping>(queryRows).Result;

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(queryRow).Result;
                var mappedRows = mapper.MapRows<BasicMapping>(queryRows).Result;

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_basic_mapping_even_though_columns_are_missing()
        {
            var mapper = GetQueryMapper();

            var queryRow = Query.Create(@"
select top 1
    Id,
    Name as NotMapped,
    Age as TheAge
from
    BasicMapping
");

            var queryRows = Query.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = mapper.MapObject<BasicMapping>(queryRow).Result;
                var items = mapper.MapObjects<BasicMapping>(queryRows).Result;

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(queryRow).Result;
                var mappedRows = mapper.MapRows<BasicMapping>(queryRows).Result;

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_value_mapping()
        {
            var mapper = GetQueryMapper();

            var queryRow = Query.Create(@"
select top 1
    Id
from
    BasicMapping
");

            var queryRows = Query.Create(@"
select
    Id
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var value = mapper.MapValue<Guid>(queryRow).Result;
                var values = mapper.MapValues<Guid>(queryRows).Result;

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }

        [Test]
        public void Should_be_able_to_perform_dynamic_mapping()
        {
            var databaseGateway = GetDatabaseGateway();
            var queryMapper = GetQueryMapper();

            var queryRow = Query.Create(@"
select top 1
    Id,
    Name,
    Age
from
    BasicMapping
");

            var queryRows = Query.Create(@"
select
    Id,
    Name,
    Age
from
    BasicMapping
");

            using (GetDatabaseContext())
            {
                var item = queryMapper.MapItem(queryRow).Result;
                var items = queryMapper.MapItems(queryRows).Result;

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());
            }
        }
    }
}