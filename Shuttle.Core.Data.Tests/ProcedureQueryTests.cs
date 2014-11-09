using System;
using System.Data;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class ProcedureQueryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_query()
		{
			const string sql = "uspDoSomething";

			var query1 = new ProcedureQuery(sql);
			var query2 = ProcedureQuery.Create(sql);
		}

		[Test]
		public void Should_be_able_prepare_a_query()
		{
			const string sql = "uspDoSomething";

			var guid = Guid.NewGuid();
			var mc = new MappedColumn<Guid>("Id", DbType.Guid);
			var query = new ProcedureQuery(sql).AddParameterValue(mc, guid);
			var dataParameterCollection = new Mock<IDataParameterCollection>();
			var dataParameterFactory = new Mock<IDbDataParameterFactory>();

			dataParameterFactory.Setup(m => m.Create("@Id", DbType.Guid, guid));

			var dataSource = new DataSource("data-source", dataParameterFactory.Object);

			var command = new Mock<IDbCommand>();

			dataParameterCollection.Setup(m => m.Add(It.IsAny<IDbDataParameter>())).Verifiable();

			command.SetupGet(m => m.Parameters).Returns(dataParameterCollection.Object);
			command.SetupSet(m => m.CommandText = sql).Verifiable();
			command.SetupSet(m => m.CommandType = CommandType.StoredProcedure).Verifiable();

			query.Prepare(dataSource, command.Object);

			command.VerifyAll();
			dataParameterFactory.VerifyAll();
		}
	}
}