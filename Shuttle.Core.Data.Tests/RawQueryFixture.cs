using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class RawQueryFixture : Fixture
    {
        [Test]
        public void Should_be_able_prepare_a_query()
        {
            const string sql = "select @Id";

            var guid = Guid.NewGuid();
            var mc = new Column<Guid>("Id", DbType.Guid);
            var query = new RawQuery(sql).AddParameterValue(mc, guid);
            var dataParameterCollection = new Mock<IDataParameterCollection>();

            var command = new Mock<IDbCommand>();

            dataParameterCollection.Setup(m => m.Add(It.IsAny<IDbDataParameter>())).Verifiable();

            command.SetupGet(m => m.Parameters).Returns(dataParameterCollection.Object);
            command.SetupSet(m => m.CommandText = sql).Verifiable();
            command.SetupSet(m => m.CommandType = CommandType.Text).Verifiable();
            command.Setup(m => m.CreateParameter()).Returns(new SqlParameter());

            query.Prepare(command.Object);

            command.VerifyAll();
        }

        [Test]
        public void Should_be_able_to_create_a_query()
        {
            const string sql = "select 1";

            var query1 = new RawQuery(sql);
            var query2 = RawQuery.Create(sql);
            var query3 = RawQuery.Create("select {0}", 1);
        }
    }
}