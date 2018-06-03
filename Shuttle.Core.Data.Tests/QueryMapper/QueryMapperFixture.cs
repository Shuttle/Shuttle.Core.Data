using System;
using System.Linq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class QueryMapperFixture : Fixture
    {
        [SetUp]
        public void SetUp()
        {
            using (GetDatabaseContext())
            {
                new DatabaseGateway().ExecuteUsing(RawQuery.Create(@"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BasicMapping]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BasicMapping](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](65) NOT NULL,
	[Age] [int] NOT NULL,
 CONSTRAINT [PK_BasicMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_BasicMapping_Id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[BasicMapping] ADD  CONSTRAINT [DF_BasicMapping_Id]  DEFAULT (newid()) FOR [Id]
END

delete from BasicMapping;

insert into BasicMapping
(
    Id,
    Name,
    Age
)
values
(
    'E09D96E1-5401-4CB6-A871-092DA1C7248D',
    'Name-1',
    25
)

insert into BasicMapping
(
    Id,
    Name,
    Age
)
values
(
    'B5E0088E-4873-4244-9B91-1059E0383C3E',
    'Name-2',
    50
)
"));
            }
        }

        [Test]
        public void Should_be_able_to_do_basic_mapping()
        {
            var mapper = new QueryMapper(new DatabaseGateway(), new DataRowMapper());

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

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(queryRow);
                var mappedRows = mapper.MapRows<BasicMapping>(queryRows);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_do_basic_mapping_even_though_columns_are_missing()
        {
            var mapper = new QueryMapper(new DatabaseGateway(), new DataRowMapper());

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

            using (GetDatabaseContext())
            {
                var item = mapper.MapObject<BasicMapping>(queryRow);
                var items = mapper.MapObjects<BasicMapping>(queryRows);

                Assert.IsNotNull(item);
                Assert.AreEqual(2, items.Count());

                var mappedRow = mapper.MapRow<BasicMapping>(queryRow);
                var mappedRows = mapper.MapRows<BasicMapping>(queryRows);

                Assert.IsNotNull(mappedRow);
                Assert.AreEqual(2, mappedRows.Count());
            }
        }

        [Test]
        public void Should_be_able_to_do_value_mapping()
        {
            var mapper = new QueryMapper(new DatabaseGateway(), new DataRowMapper());

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

            using (GetDatabaseContext())
            {
                var value = mapper.MapValue<Guid>(queryRow);
                var values = mapper.MapValues<Guid>(queryRows);

                Assert.IsNotNull(value);
                Assert.AreEqual(2, values.Count());
            }
        }
    }
}