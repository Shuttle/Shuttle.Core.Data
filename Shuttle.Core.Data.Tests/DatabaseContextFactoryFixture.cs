﻿using System;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DatabaseContextFactoryFixture : Fixture
{
    [Test]
    public void Should_be_able_to_create_a_database_context()
    {
        using (var context = DatabaseContextFactory.Create(DefaultConnectionStringName))
        {
            Assert.That(context, Is.Not.Null);
        }
    }


    [Test]
    public void Should_be_able_to_check_connection_availability()
    {
        var databaseContextFactory = new Mock<IDatabaseContextFactory>();

        Assert.That(databaseContextFactory.Object.IsAvailable(new(), 0, 0), Is.True);
        Assert.That(databaseContextFactory.Object.IsAvailable("name", new(), 0, 0), Is.True);

        databaseContextFactory.Setup(m => m.Create()).Throws(new Exception());
        databaseContextFactory.Setup(m => m.Create(It.IsAny<string>())).Throws(new Exception());

        Assert.That(databaseContextFactory.Object.IsAvailable(new(), 0, 0), Is.False);
        Assert.That(databaseContextFactory.Object.IsAvailable("name", new(), 0, 0), Is.False);
    }
}