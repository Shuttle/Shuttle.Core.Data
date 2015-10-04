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
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var table = gateway.GetDataTableFor(GetDatabaseConnection(), query.Object);

				Assert.IsNotNull(table);
				Assert.AreEqual(2, table.Rows.Count);
				Assert.AreEqual("row-1", table.Rows[0][0]);
				Assert.AreEqual("row-2", table.Rows[1][0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_rows()
		{
			var query = new Mock<IQuery>();

			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var rows = gateway.GetRowsUsing(GetDatabaseConnection(), query.Object).ToList();

				Assert.IsNotNull(rows);
				Assert.AreEqual(2, rows.Count());
				Assert.AreEqual("row-1", rows[0][0]);
				Assert.AreEqual("row-2", rows[1][0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_single_row()
		{
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(2));

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var row = gateway.GetSingleRowUsing(GetDatabaseConnection(), query.Object);

				Assert.IsNotNull(row);
				Assert.AreEqual("row-1", row[0]);
			}
		}

		[Test]
		public void Should_be_able_to_get_null_for_single_row_if_there_is_no_data()
		{
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteReader()).Returns(DataTableReader(0));

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				Assert.IsNull(gateway.GetSingleRowUsing(GetDatabaseConnection(), query.Object));
			}
		}

		[Test]
		public void Should_be_able_to_execute_a_query()
		{
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteNonQuery()).Returns(1);

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var result = gateway.ExecuteUsing(GetDatabaseConnection(), query.Object);

				Assert.IsNotNull(result);
				Assert.AreEqual(1, result);
			}
		}

		[Test]
		public void Should_be_able_to_get_a_scalar()
		{
			var query = new Mock<IQuery>();
			var command = CommandMock();

			command.Setup(m => m.ExecuteScalar()).Returns(10);

			var gateway = new DatabaseGateway();

			var log = new Mock<ILog>();

			log.Setup(m => m.IsTraceEnabled).Returns(true);

			using (Log.AssignTransient(log.Object))
			{
				var result = gateway.GetScalarUsing<int>(GetDatabaseConnection(), query.Object);

				Assert.IsNotNull(result);
				Assert.AreEqual(10, result);
			}
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