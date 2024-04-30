using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class ProcedureQueryFixture : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_query()
		{
			const string sql = "uspDoSomething";

			var query1 = new Query(sql, CommandType.StoredProcedure);
		}

		[Test]
		public void Should_be_able_prepare_a_query()
		{
			const string sql = "uspDoSomething";

			var guid = Guid.NewGuid();
			var mc = new Column<Guid>("Id", DbType.Guid);
			var query = new Query(sql, CommandType.StoredProcedure).AddParameter(mc, guid);
			var dataParameterCollection = new Mock<IDataParameterCollection>();

			var command = new Mock<IDbCommand>();

			dataParameterCollection.Setup(m => m.Add(It.IsAny<IDbDataParameter>())).Verifiable();

			command.SetupGet(m => m.Parameters).Returns(dataParameterCollection.Object);
			command.SetupSet(m => m.CommandText = sql).Verifiable();
			command.SetupSet(m => m.CommandType = CommandType.StoredProcedure).Verifiable();
			command.Setup(m => m.CreateParameter()).Returns(new SqlParameter());

			query.Prepare(command.Object);

			command.VerifyAll();
		}
	}
}