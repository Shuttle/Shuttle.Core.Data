using System.Linq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class QueryMapperFixture : Fixture
    {
        [Test]
        public void Should_be_able_to_do_basic_mapping()
        {
            var mapper = new QueryMapper(new DatabaseGateway());

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

            using (GetDatabaseContext())
            {
                var item = mapper.MapObject<BasicMapping>(queryRow);
                var items = mapper.MapObjects<BasicMapping>(queryRows);

                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(queryRow);
                var mappedRows = mapper.MapRows<BasicMapping>(queryRows);

                Assert.AreEqual(2, mappedRows.Count());
            }
        }
    }
}