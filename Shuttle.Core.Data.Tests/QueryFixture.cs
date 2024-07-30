using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class QueryFixture : Fixture
{
    [Test]
    public void Should_be_able_prepare_a_text_query()
    {
        const string sql = "select @Id";

        var guid = Guid.NewGuid();
        var mc = new Column<Guid>("Id", DbType.Guid);
        var query = new Query(sql).AddParameter(mc, guid);
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
    public void Should_be_able_to_create_a_text_query()
    {
        Assert.That(() => new Query("select 1"), Throws.Nothing);
    }

    [Test]
    public void Should_be_able_to_create_a_stored_procedure_query()
    {
        Assert.That(() => new Query("uspDoSomething", CommandType.StoredProcedure), Throws.Nothing);
    }

    [Test]
    public void Should_be_able_prepare_a_stored_procedure_query()
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