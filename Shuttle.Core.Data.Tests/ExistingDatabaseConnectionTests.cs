using System.Data;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class ExistingDatabaseConnectionTests : Fixture
	{
		[Test]
		public void Should_be_able_to_create_a_valid_connection()
		{
			var dataSource = DefaultDataSource();

			using (new ExistingDatabaseConnection(
				new DatabaseConnection(dataSource,
				                       DbConnectionFactory.Default().CreateConnection(dataSource),
				                       new Mock<IDbCommandFactory>().Object,
				                       new Mock<IDatabaseConnectionCache>().Object)))
			{
			}
		}

		[Test]
		public void Should_be_able_to_begin_and_commit_a_transaction()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new ExistingDatabaseConnection(
					new DatabaseConnection(dataSource,
					                       DbConnectionFactory.Default().CreateConnection(dataSource),
					                       new Mock<IDbCommandFactory>().Object,
					                       new Mock<IDatabaseConnectionCache>().Object))
				)
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
				var connection = new ExistingDatabaseConnection(
					new DatabaseConnection(dataSource,
					                       DbConnectionFactory.Default().CreateConnection(dataSource),
					                       new Mock<IDbCommandFactory>().Object,
					                       new Mock<IDatabaseConnectionCache>().Object))
				)
			{
				connection.BeginTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_commit_without_a_transaction()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new ExistingDatabaseConnection(
					new DatabaseConnection(dataSource,
					                       DbConnectionFactory.Default().CreateConnection(dataSource),
					                       new Mock<IDbCommandFactory>().Object,
					                       new Mock<IDatabaseConnectionCache>().Object))
				)
			{
				connection.CommitTransaction();
			}
		}

		[Test]
		public void Should_be_able_to_call_dispose_more_than_once()
		{
			var dataSource = DefaultDataSource();

			using (
				var connection = new ExistingDatabaseConnection(
					new DatabaseConnection(dataSource,
					                       DbConnectionFactory.Default().CreateConnection(dataSource),
					                       new Mock<IDbCommandFactory>().Object,
					                       new Mock<IDatabaseConnectionCache>().Object))
				)
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
				var connection = new ExistingDatabaseConnection(
					new DatabaseConnection(dataSource,
					                       dbConnection,
					                       dbCommandFactory.Object,
					                       new Mock<IDatabaseConnectionCache>().Object))
				)
			{
				connection.CreateCommandToExecute(query.Object);
			}

			dbCommandFactory.VerifyAll();
		}
	}
}