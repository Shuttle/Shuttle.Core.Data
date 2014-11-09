using System;
using System.Data;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class RawQueryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_query()
		{
			const string sql = "select 1";

			var query1 = new RawQuery(sql);
			var query2 = RawQuery.Create(sql);
			var query3 = RawQuery.Create("select {0}", 1);
		}

		[Test]
		public void Should_be_able_prepare_a_query()
		{
			const string sql = "select @Id";

			var guid = Guid.NewGuid();
			var mc = new MappedColumn<Guid>("Id", DbType.Guid);
			var query = new RawQuery(sql).AddParameterValue(mc, guid);
			var dataParameterCollection = new Mock<IDataParameterCollection>();
			var dataParameterFactory = new Mock<IDbDataParameterFactory>();

			dataParameterFactory.Setup(m => m.Create("@Id", DbType.Guid, guid));

			var dataSource = new DataSource("data-source", dataParameterFactory.Object);

			var command = new Mock<IDbCommand>();

			dataParameterCollection.Setup(m => m.Add(It.IsAny<IDbDataParameter>())).Verifiable();

			command.SetupGet(m => m.Parameters).Returns(dataParameterCollection.Object);
			command.SetupSet(m => m.CommandText = sql).Verifiable();
			command.SetupSet(m => m.CommandType = CommandType.Text).Verifiable();

			query.Prepare(dataSource, command.Object);

			command.VerifyAll();
			dataParameterFactory.VerifyAll();
		}
	}
}