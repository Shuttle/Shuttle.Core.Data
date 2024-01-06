using System.Threading.Tasks;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

public class MappingFixture : Fixture
{
    [SetUp]
    public async Task SetUp()
    {
        using (GetDatabaseContext())
        {
            await GetDatabaseGateway().ExecuteAsync(new Query(@"
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
),
(
    'B5E0088E-4873-4244-9B91-1059E0383C3E',
    'Name-2',
    50
)
"));
        }
    }
}