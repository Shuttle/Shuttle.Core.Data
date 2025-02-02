using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DbCommandFactoryFixture : Fixture
{
    [Test]
    public void Should_be_able_to_create_a_command()
    {
        DbCommandFactory factory = new(Options.Create(new DataAccessOptions { CommandTimeout = 15 }));
        Mock<IDbConnection> connection = new();
        Mock<IQuery> query = new();
        Mock<DbCommand> command = new();

        command.SetupSet(m => m.CommandTimeout = 15).Verifiable("CommandTimeout not set to 15");

        connection.Setup(m => m.CreateCommand()).Returns(command.Object);
        query.Setup(m => m.Prepare(command.Object));

        var result = factory.Create(connection.Object, query.Object);

        connection.VerifyAll();
        query.VerifyAll();

        Assert.That(command.Object, Is.SameAs(result));
    }
}