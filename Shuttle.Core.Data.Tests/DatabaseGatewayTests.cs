using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DatabaseGatewayTests : Fixture
	{
		[Test]
		public void Should_be_able_to_get_a_data_table()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var table = gateway.GetDataTableFor(dataSource, query.Object);

				Assert.IsNotNull(table);
				Assert.AreEqual(2, table.Rows.Count);
				Assert.AreEqual("row-1", table.Rows[0][0]);
				Assert.AreEqual("row-2", table.Rows[1][0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_rows()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();

			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var rows = gateway.GetRowsUsing(dataSource, query.Object).ToList();

				Assert.IsNotNull(rows);
				Assert.AreEqual(2, rows.Count());
				Assert.AreEqual("row-1", rows[0][0]);
				Assert.AreEqual("row-2", rows[1][0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_single_row()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var row = gateway.GetSingleRowUsing(dataSource, query.Object);

				Assert.IsNotNull(row);
				Assert.AreEqual("row-1", row[0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_null_for_single_row_if_there_is_no_data()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(0));

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				Assert.IsNull(gateway.GetSingleRowUsing(dataSource, query.Object));
			}
		}

		[Test]
		public void Should_be_able_to_execute_a_query()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteNonQuery()).Returns(1);

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var result = gateway.ExecuteUsing(dataSource, query.Object);

				Assert.IsNotNull(result);
				Assert.AreEqual(1, result);
			}
		}

		[Test]
		public void Should_be_able_to_get_a_scalar()
		{
			var dataSource = DefaultDataSource();
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteScalar()).Returns(10);

			var gateway = CreateDatabaseGateway(dataSource, query, command);

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var result = gateway.GetScalarUsing<int>(dataSource, query.Object);

				Assert.IsNotNull(result);
				Assert.AreEqual(10, result);
			}
		}

		[Test]
		[ExpectedException(typeof (NullReferenceException))]
		public void Should_be_able_to_catch_exception()
		{
			var dataSource = DefaultDataSource();
			var databaseConnectionCache = new Mock<IDatabaseConnectionCache>();

			databaseConnectionCache.Setup(m => m.Get(dataSource)).Returns((IDatabaseConnection) null);

			var gateway = new DatabaseGateway(databaseConnectionCache.Object);

			gateway.GetDataTableFor(dataSource, new Mock<IQuery>().Object);
		}

		[Test]
		public void Should_be_able_to_create_default_database_database_gateway()
		{
			Assert.IsNotNull(DatabaseGateway.Default());
		}

		private DataTableReader DataTableReader(int rowCount)
		{
			var table = new DataTable();

			table.Columns.Add("object");

			for (var i = 0; i < rowCount; i++)
			{
				table.Rows.Add(string.Concat("row-", i + 1));
			}

			return new DataTableReader(table);
		}

		private DatabaseGateway CreateDatabaseGateway(DataSource dataSource, IMock<IQuery> query, IMock<IDbCommand> command)
		{
			var databaseConnectionCache = new Mock<IDatabaseConnectionCache>();
			var databaseConnection = new Mock<IDatabaseConnection>();

			databaseConnectionCache.Setup(m => m.Get(dataSource)).Returns(databaseConnection.Object);
			databaseConnection.Setup(m => m.CreateCommandToExecute(query.Object)).Returns(command.Object);

			return new DatabaseGateway(databaseConnectionCache.Object);
		}

		private Mock<IDbCommand> CommandMock()
		{
			var dbCommand = new Mock<IDbCommand>();
			var dataParameterCollection = new Mock<IDataParameterCollection>();
			var dataParameter = new Mock<IDataParameter>();

			dataParameter.Setup(m => m.ParameterName).Returns("some-parameter");
			dataParameter.Setup(m => m.Value).Returns("some-value");
			dataParameterCollection.Setup(m => m.GetEnumerator())
			                       .Returns(new List<IDataParameter> {dataParameter.Object}.GetEnumerator());

			dbCommand.Setup(m => m.CommandType).Returns(CommandType.Text);
			dbCommand.Setup(m => m.CommandText).Returns("some-sql");
			dbCommand.Setup(m => m.Parameters).Returns(dataParameterCollection.Object);

			return dbCommand;
		}
	}
}