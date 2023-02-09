using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseContextFixture : Fixture
	{
		[Test]
		public void Should_not_be_able_to_create_an_invalid_connection()
		{
		    Assert.Throws<ArgumentException>(() =>
		    {
		        using (new DatabaseContext("Microsoft	.Data.SqlClient", new SqlConnection("```"), new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
		        {
		        }
		    });
		}

		[Test]
		public void Should_not_be_able_to_create_a_non_existent_connection()
		{
		    Assert.Throws<SqlException>(() =>
		    {
		        using (
		            new DatabaseContext("Microsoft.Data.SqlClient", new SqlConnection("data source=.;initial catalog=idontexist;integrated security=sspi"),
		                new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
		        {
		        }
		    });
		}

		[Test]
		public void Should_be_able_to_create_a_valid_connection()
		{
			using (
				new DatabaseContext("Microsoft.Data.SqlClient", GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString),
					new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
			{
			}
		}

        [Test]
		public void Should_be_able_to_begin_and_commit_a_transaction()
		{
			using (
				var connection =
					new DatabaseContext("Microsoft.Data.SqlClient", GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
			{
				connection.BeginTransaction();
				connection.CommitTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_begin_and_rollback_a_transaction()
		{
			using (
				var connection =
					new DatabaseContext("Microsoft.Data.SqlClient", GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
			{
				connection.BeginTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_commit_without_a_transaction()
		{
			using (
				var connection =
					new DatabaseContext("Microsoft.Data.SqlClient", GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
			{
				connection.CommitTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_dispose_more_than_once()
		{
			using (
				var connection =
					new DatabaseContext("Microsoft.Data.SqlClient", GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object, new DatabaseContextService()))
			{
				connection.Dispose();
				connection.Dispose();
			}
		}

		[Test]
		public void Should_be_able_to_create_a_command()
		{
			var dbCommandFactory = new Mock<IDbCommandFactory>();
			var dbConnection = GetDbConnectionFactory().Create(DefaultProviderName, DefaultConnectionString);
			var query = new Mock<IQuery>();
			var dbCommand = new Mock<DbCommand>();

			dbCommandFactory.Setup(m => m.Create(dbConnection, query.Object)).Returns(dbCommand.Object);

			using (
				var connection = new DatabaseContext("Microsoft.Data.SqlClient", dbConnection, dbCommandFactory.Object, new DatabaseContextService()))
			{
				connection.CreateCommand(query.Object);
			}

			dbCommandFactory.VerifyAll();
		}
	}
}