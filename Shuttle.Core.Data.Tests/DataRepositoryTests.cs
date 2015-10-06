using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DataRepositoryTests : Fixture
	{
		[Test]
		public void Should_be_able_to_fetch_all_items()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetRowsUsing(query.Object)).Returns(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchAllUsing(query.Object).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(anObject, result[0]);
		}

		[Test]
		public void Should_be_able_to_fetch_a_single_item()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetSingleRowUsing(query.Object)).Returns(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchItemUsing(query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(anObject, result);
		}

		[Test]
		public void Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetSingleRowUsing(query.Object)).Returns((DataRow) null);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			var result = repository.FetchItemUsing(query.Object);

			Assert.IsNull(result);
		}

		[Test]
		public void Should_be_able_to_call_contains()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetScalarUsing<int>(query.Object)).Returns(1);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			Assert.IsTrue(repository.Contains(query.Object));
		}

		[Test]
		public void Should_be_able_to_fetch_mapped_rows()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetRowsUsing(query.Object)).Returns(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchMappedRowsUsing(query.Object).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(dataRow, result[0].Row);
			Assert.AreSame(anObject, result[0].Result);
		}

		[Test]
		public void Should_be_able_to_fetch_a_single_row()
		{
			var connection = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetSingleRowUsing(query.Object)).Returns(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchMappedRowUsing(query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(dataRow, result.Row);
			Assert.AreSame(anObject, result.Result);
		}
	}
}