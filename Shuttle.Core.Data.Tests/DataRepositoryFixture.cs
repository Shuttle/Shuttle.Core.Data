using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class DataRepositoryFixture : Fixture
	{
		[Test]
		public async Task Should_be_able_to_fetch_all_items()
		{
			var context = new Mock<IDatabaseContext>();
			var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetRows(context.Object, query.Object, CancellationToken.None)).ReturnsAsync(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

            var result = (await repository.FetchItems(context.Object, query.Object)).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(anObject, result[0]);
		}

		[Test]
		public async Task Should_be_able_to_fetch_a_single_item()
		{
			var context = new Mock<IDatabaseContext>();
            var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();

			gateway.Setup(m => m.GetRow(context.Object, query.Object, CancellationToken.None)).ReturnsAsync(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = await repository.FetchItem(context.Object, query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(anObject, result);
		}

		[Test]
		public async Task Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found()
		{
			var context = new Mock<IDatabaseContext>();
            var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetRow(context.Object, query.Object, CancellationToken.None)).ReturnsAsync((DataRow) null);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			var result = await repository.FetchItem(context.Object, query.Object);

			Assert.IsNull(result);
		}

		[Test]
		public async Task Should_be_able_to_call_contains()
		{
			var context = new Mock<IDatabaseContext>();
            var gateway = new Mock<IDatabaseGateway>();
			var query = new Mock<IQuery>();

			gateway.Setup(m => m.GetScalar<int>(context.Object, query.Object, CancellationToken.None)).ReturnsAsync(1);

			var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

			Assert.That(await repository.Contains(context.Object, query.Object), Is.True);
		}

		[Test]
		public async Task Should_be_able_to_fetch_mapped_rows()
		{
			var context = new Mock<IDatabaseContext>();
            var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetRows(context.Object, query.Object, CancellationToken.None)).ReturnsAsync(new List<DataRow> {dataRow});
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = (await repository.FetchMappedRows(context.Object, query.Object)).ToList();

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreSame(dataRow, result[0].Row);
			Assert.AreSame(anObject, result[0].Result);
		}

		[Test]
		public async Task Should_be_able_to_fetch_a_single_row()
		{
			var context = new Mock<IDatabaseContext>();
            var gateway = new Mock<IDatabaseGateway>();
			var mapper = new Mock<IDataRowMapper<object>>();
			var query = new Mock<IQuery>();
			var dataRow = new DataTable().NewRow();
			var anObject = new object();
			var mappedRow = new MappedRow<object>(dataRow, anObject);

			gateway.Setup(m => m.GetRow(context.Object, query.Object, CancellationToken.None)).ReturnsAsync(dataRow);
			mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

			var repository = new DataRepository<object>(gateway.Object, mapper.Object);

			var result = await repository.FetchMappedRow(context.Object, query.Object);

			Assert.IsNotNull(result);
			Assert.AreSame(dataRow, result.Row);
			Assert.AreSame(anObject, result.Result);
		}
	}
}