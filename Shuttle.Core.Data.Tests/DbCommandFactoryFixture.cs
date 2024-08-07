﻿using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
    [TestFixture]
    public class DbCommandFactoryFixture : Fixture
    {
        [Test]
        public void Should_be_able_to_create_a_command()
        {
            var factory = new DbCommandFactory(Options.Create(new DataAccessOptions { CommandTimeout = 15 }));
            var connection = new Mock<IDbConnection>();
            var query = new Mock<IQuery>();
            var command = new Mock<DbCommand>();

            command.SetupSet(m => m.CommandTimeout = 15).Verifiable("CommandTimeout not set to 15");

            connection.Setup(m => m.CreateCommand()).Returns(command.Object);
            query.Setup(m => m.Prepare(command.Object));

            var result = factory.Create(connection.Object, query.Object);

            connection.VerifyAll();
            query.VerifyAll();

            Assert.AreSame(result, command.Object);
        }
    }
}