using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class DataRepositoryFixture : Fixture
{
    [Test]
    public void Should_be_able_to_fetch_all_items()
    {
        Should_be_able_to_fetch_all_items_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_fetch_all_items_async()
    {
        await Should_be_able_to_fetch_all_items_async(false);
    }

    private async Task Should_be_able_to_fetch_all_items_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var mapper = new Mock<IDataRowMapper<object>>();
        var query = new Mock<IQuery>();
        var dataRow = new DataTable().NewRow();
        var anObject = new object();

        gateway.Setup(m => m.GetRows(query.Object, CancellationToken.None)).Returns(new List<DataRow> { dataRow });
        gateway.Setup(m => m.GetRowsAsync(query.Object, CancellationToken.None)).ReturnsAsync(new List<DataRow> { dataRow });
        mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

        var repository = new DataRepository<object>(gateway.Object, mapper.Object);

        var result = (sync
            ? repository.FetchItems(query.Object)
            : await repository.FetchItemsAsync(query.Object)).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreSame(anObject, result[0]);
    }

    [Test]
    public void Should_be_able_to_fetch_a_single_item()
    {
        Should_be_able_to_fetch_a_single_item_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_fetch_a_single_item_async()
    {
        await Should_be_able_to_fetch_a_single_item_async(false);
    }

    private async Task Should_be_able_to_fetch_a_single_item_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var mapper = new Mock<IDataRowMapper<object>>();
        var query = new Mock<IQuery>();
        var dataRow = new DataTable().NewRow();
        var anObject = new object();

        gateway.Setup(m => m.GetRowAsync(query.Object, CancellationToken.None)).ReturnsAsync(dataRow);
        mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(new MappedRow<object>(dataRow, anObject));

        var repository = new DataRepository<object>(gateway.Object, mapper.Object);

        var result = await repository.FetchItemAsync(query.Object);

        Assert.IsNotNull(result);
        Assert.AreSame(anObject, result);
    }

    [Test]
    public async Task Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found()
    {
        Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found_async()
    {
        await Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found_async(false);
    }

    private async Task Should_be_able_to_get_default_when_fetching_a_single_item_that_is_not_found_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var query = new Mock<IQuery>();

        gateway.Setup(m => m.GetRow(query.Object, CancellationToken.None)).Returns((DataRow)null);
        gateway.Setup(m => m.GetRowAsync(query.Object, CancellationToken.None)).ReturnsAsync((DataRow)null);

        var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

        var result = sync
            ? repository.FetchItem(query.Object)
            : await repository.FetchItemAsync(query.Object);

        Assert.IsNull(result);
    }

    [Test]
    public void Should_be_able_to_call_contains()
    {
        Should_be_able_to_call_contains_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_call_contains_async()
    {
        await Should_be_able_to_call_contains_async(false);
    }

    public async Task Should_be_able_to_call_contains_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var query = new Mock<IQuery>();

        gateway.Setup(m => m.GetScalar<int>(query.Object, CancellationToken.None)).Returns(1);
        gateway.Setup(m => m.GetScalarAsync<int>(query.Object, CancellationToken.None)).ReturnsAsync(1);

        var repository = new DataRepository<object>(gateway.Object, new Mock<IDataRowMapper<object>>().Object);

        Assert.That(sync ? repository.Contains(query.Object) : await repository.ContainsAsync(query.Object), Is.True);
    }

    [Test]
    public void Should_be_able_to_fetch_mapped_rows()
    {
        Should_be_able_to_fetch_mapped_rows_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_fetch_mapped_rows_async()
    {
        await Should_be_able_to_fetch_mapped_rows_async(false);
    }

    private async Task Should_be_able_to_fetch_mapped_rows_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var mapper = new Mock<IDataRowMapper<object>>();
        var query = new Mock<IQuery>();
        var dataRow = new DataTable().NewRow();
        var anObject = new object();
        var mappedRow = new MappedRow<object>(dataRow, anObject);

        gateway.Setup(m => m.GetRows(query.Object, CancellationToken.None)).Returns(new List<DataRow> { dataRow });
        gateway.Setup(m => m.GetRowsAsync(query.Object, CancellationToken.None)).ReturnsAsync(new List<DataRow> { dataRow });
        mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

        var repository = new DataRepository<object>(gateway.Object, mapper.Object);

        var result = (sync ? repository.FetchMappedRows(query.Object) : await repository.FetchMappedRowsAsync(query.Object)).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreSame(dataRow, result[0].Row);
        Assert.AreSame(anObject, result[0].Result);
    }

    [Test]
    public async Task Should_be_able_to_fetch_a_single_row()
    {
        Should_be_able_to_fetch_a_single_row_async(true).GetAwaiter().GetResult();
    }

    [Test]
    public async Task Should_be_able_to_fetch_a_single_row_async()
    {
        await Should_be_able_to_fetch_a_single_row_async(false);
    }

    private async Task Should_be_able_to_fetch_a_single_row_async(bool sync)
    {
        var gateway = new Mock<IDatabaseGateway>();
        var mapper = new Mock<IDataRowMapper<object>>();
        var query = new Mock<IQuery>();
        var dataRow = new DataTable().NewRow();
        var anObject = new object();
        var mappedRow = new MappedRow<object>(dataRow, anObject);

        gateway.Setup(m => m.GetRow(query.Object, CancellationToken.None)).Returns(dataRow);
        gateway.Setup(m => m.GetRowAsync(query.Object, CancellationToken.None)).ReturnsAsync(dataRow);
        mapper.Setup(m => m.Map(It.IsAny<DataRow>())).Returns(mappedRow);

        var repository = new DataRepository<object>(gateway.Object, mapper.Object);

        var result = sync
            ? repository.FetchMappedRow(query.Object)
            : await repository.FetchMappedRowAsync(query.Object);

        Assert.IsNotNull(result);
        Assert.AreSame(dataRow, result.Row);
        Assert.AreSame(anObject, result.Result);
    }
}