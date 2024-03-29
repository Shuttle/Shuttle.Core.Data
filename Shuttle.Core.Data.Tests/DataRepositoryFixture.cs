﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DataRepositoryFixture : Fixture
	{
		[Test]
		public void Should_be_able_to_fetch_all_items()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetRows(query.Object)).Returns(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchItems(query.Object).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(anObject, result[0]);
		}

		[Test]
		public void Should_be_able_to_fetch_a_single_item()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetRow(query.Object)).Returns(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchItem(query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(anObject, result);
		}

		[Test]
		public void Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetRow(query.Object)).Returns((DataRow) null);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			var result = repository.FetchItem(query.Object);

			Assert.IsNull(result);
		}

		[Test]
		public void Should_be_able_to_call_contains()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetScalar<int>(query.Object)).Returns(1);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			Assert.IsTrue(repository.Contains(query.Object));
		}

		[Test]
		public void Should_be_able_to_fetch_mapped_rows()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetRows(query.Object)).Returns(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchMappedRows(query.Object).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(dataRow, result[0].Row);
			Assert.AreSame(anObject, result[0].Result);
		}

		[Test]
		public void Should_be_able_to_fetch_a_single_row()
		{
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetRow(query.Object)).Returns(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = repository.FetchMappedRow(query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(dataRow, result.Row);
			Assert.AreSame(anObject, result.Result);
		}
	}
}