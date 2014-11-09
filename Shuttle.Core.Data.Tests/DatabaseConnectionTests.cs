using System;
using System.Data;
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseConnectionTests : Fixture
	{
		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void Should_not_be_able_to_create_an_invalid_connection()
		{
			var dataSource = DefaultDataSource();

			using (new DatabaseConnection(dataSource,
			                              new SqlConnection("~~~"),
			                              new Mock<IDbCommandFactory>().Object,
			                              new Mock<IDatabaseConnectionCache>().Object))
			{
			}
		}

		[Test]
		[ExpectedException(typeof (SqlException))]
		public void Should_not_be_able_to_create_a_non_existent_connection()
		{
			var dataSource = DefaultDataSource();

			using (new DatabaseConnection(dataSource,
			                              new SqlConnection("data source=.;initial catalog=idontexist;integrated security=sspi"),
			                              new Mock<IDbCommandFactory>().Object,
			                              new Mock<IDatabaseConnectionCache>().Object))
			{
			}
		}

		[Test]
		public void Should_be_able_to_create_a_valid_connection()
		{
			var dataSource = DefaultDataSource();

			using (new DatabaseConnection(dataSource,
			                              DbConnectionFactory.Default().CreateConnection(dataSource),
			                              new Mock<IDbCommandFactory>().Object,
			                              new Mock<IDatabaseConnectionCache>().Object))
			{
			}
		}

		[Test]
		public void Should_be_able_to_begin_and_commit_a_transaction()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new DatabaseConnection(dataSource,
				                                        DbConnectionFactory.Default().CreateConnection(dataSource),
				                                        new Mock<IDbCommandFactory>().Object,
				                                        new Mock<IDatabaseConnectionCache>().Object))
			{
				connection.BeginTransaction();
				connection.CommitTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_begin_and_rollback_a_transaction()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new DatabaseConnection(dataSource,
				                                        DbConnectionFactory.Default().CreateConnection(dataSource),
				                                        new Mock<IDbCommandFactory>().Object,
				                                        new Mock<IDatabaseConnectionCache>().Object))
			{
				connection.BeginTransaction();
			}
		}
		
		[Test]
		public void Should_be_able_to_call_commit_without_a_transaction()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new DatabaseConnection(dataSource,
				                                        DbConnectionFactory.Default().CreateConnection(dataSource),
				                                        new Mock<IDbCommandFactory>().Object,
				                                        new Mock<IDatabaseConnectionCache>().Object))
			{
				connection.CommitTransaction();
			}
		}
		
		[Test]
		public void Should_be_able_to_call_dispose_more_than_once()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new DatabaseConnection(dataSource,
				                                        DbConnectionFactory.Default().CreateConnection(dataSource),
				                                        new Mock<IDbCommandFactory>().Object,
				                                        new Mock<IDatabaseConnectionCache>().Object))
			{
				connection.Dispose();
				connection.Dispose();
			}
		}
		
		[Test]
		public void Should_be_able_to_create_a_command()
		{
			var dataSource = DefaultDataSource();

			var dbCommandFactory = new Mock<IDbCommandFactory>();
			var dbConnection = DbConnectionFactory.Default().CreateConnection(dataSource);
			var query = new Mock<IQuery>();
			var dbCommand = new Mock<IDbCommand>();

			dbCommandFactory.Setup(m => m.CreateCommandUsing(dataSource, dbConnection, query.Object)).Returns(dbCommand.Object);

			using (
				var connection = new DatabaseConnection(dataSource,
				                                        dbConnection,
														dbCommandFactory.Object,
				                                        new Mock<IDatabaseConnectionCache>().Object))
			{
				connection.CreateCommandToExecute(query.Object);
			}

			dbCommandFactory.VerifyAll();
		}
	}
}