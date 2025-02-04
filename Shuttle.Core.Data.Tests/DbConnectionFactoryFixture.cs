﻿using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DbConnectionFactoryFixture : Fixture
{
    [Test]
    public void Should_be_able_to_create_a_valid_connection()
    {
        using (var connection = DbConnectionFactory.Create(DefaultProviderName, DefaultConnectionString))
        {
            Assert.That(connection, Is.Not.Null);
        }
    }
}