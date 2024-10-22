using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shuttle.Core.Data.Tests.Fakes;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class AssemblerExtensionsFixture
{
    [Test]
    public async Task Should_be_able_to_assemble_item_async()
    {
        var now = DateTime.Now;
        MappedData mappedData = new();

        DataTable orderTable = new();

        orderTable.Columns.Add("OrderNumber", typeof(string));
        orderTable.Columns.Add("OrderDate", typeof(DateTime));

        const string orderNumber = "ON-10";

        mappedData.Add(new MappedRow<Order>(orderTable.Rows.Add(orderNumber, now), new(orderNumber, now)));

        DataTable orderLineTable = new();

        orderLineTable.Columns.Add("OrderNumber", typeof(string));
        orderLineTable.Columns.Add("ProductId", typeof(string));
        orderLineTable.Columns.Add("Quantity", typeof(int));
        orderLineTable.Columns.Add("UnitCost", typeof(double));

        mappedData.Add(new List<MappedRow<OrderLine>> { new(orderLineTable.Rows.Add(orderNumber, "SKU-1", 5, 10), new("SKU-1", 5, 10)), new(orderLineTable.Rows.Add(orderNumber, "SKU-2", 1, 65), new("SKU-2", 1, 65)), new(orderLineTable.Rows.Add(orderNumber, "SKU-3", 10, 10.5), new("SKU-3", 10, 10.5)) });

        var order = (await new OrderAssembler().AssembleAsync(mappedData)).First();

        Assert.That(order.OrderNumber, Is.EqualTo(orderNumber));
        Assert.That(order.OrderDate, Is.EqualTo(now));
        Assert.That(order.Lines.Count(), Is.EqualTo(3));
        Assert.That(order.Lines.ElementAt(0).TotalCost(), Is.EqualTo(50));
        Assert.That(order.Lines.ElementAt(1).TotalCost(), Is.EqualTo(65));
        Assert.That(order.Lines.ElementAt(2).TotalCost(), Is.EqualTo(105));
        Assert.That(order.Total(), Is.EqualTo(220));
    }
}