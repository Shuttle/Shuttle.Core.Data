using System;
using System.Data;
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseContextTests : Fixture
	{
		[Test]
		public void Should_not_be_able_to_create_an_invalid_connection()
		{
		    Assert.Throws<ArgumentException>(() =>
		    {
		        using (new DatabaseContext("System.Data.SqlClient", new SqlConnection("~~~"), new Mock<IDbCommandFactory>().Object))
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
		            new DatabaseContext("System.Data.SqlClient", new SqlConnection("data source=.;initial catalog=idontexist;integrated security=sspi"),
		                new Mock<IDbCommandFactory>().Object))
		        {
		        }
		    });
		}

		[Test]
		public void Should_be_able_to_create_a_valid_connection()
		{
			using (
				new DatabaseContext("System.Data.SqlClient", new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString),
					new Mock<IDbCommandFactory>().Object))
			{
			}
		}

		[Test]
		public void Should_be_able_to_begin_and_commit_a_transaction()
		{
			using (
				var connection =
					new DatabaseContext("System.Data.SqlClient", new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object))
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
					new DatabaseContext("System.Data.SqlClient", new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object))
			{
				connection.BeginTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_commit_without_a_transaction()
		{
			using (
				var connection =
					new DatabaseContext("System.Data.SqlClient", new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object))
			{
				connection.CommitTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_dispose_more_than_once()
		{
			using (
				var connection =
					new DatabaseContext("System.Data.SqlClient", new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString),
						new Mock<IDbCommandFactory>().Object))
			{
				connection.Dispose();
				connection.Dispose();
			}
		}

		[Test]
		public void Should_be_able_to_create_a_command()
		{
			var dbCommandFactory = new Mock<IDbCommandFactory>();
			var dbConnection = new DbConnectionFactory().CreateConnection(DefaultProviderName, DefaultConnectionString);
			var query = new Mock<IQuery>();
			var dbCommand = new Mock<IDbCommand>();

			dbCommandFactory.Setup(m => m.CreateCommandUsing(dbConnection, query.Object)).Returns(dbCommand.Object);

			using (
				var connection = new DatabaseContext("System.Data.SqlClient", dbConnection,
					dbCommandFactory.Object))
			{
				connection.CreateCommandToExecute(query.Object);
			}

			dbCommandFactory.VerifyAll();
		}
	}
}