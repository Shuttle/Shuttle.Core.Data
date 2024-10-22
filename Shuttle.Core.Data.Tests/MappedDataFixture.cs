using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Shuttle.Core.Data.Tests.Fakes;

namespace Shuttle.Core.Data.Tests;

[TestFixture]
public class MappedDataFixture
{
    [Test]
    public void Should_be_able_to_add_and_overwrite_a_single_mapped_row()
    {
        MappedData data = new();
        var row1 = CreateDataRow();
        var row2 = CreateDataRow();
        Order order1 = new("ON-1", DateTime.Now);
        Order order2 = new("ON-2", DateTime.Now);

        data.Add(new MappedRow<Order>(row1, order1));

        var mappedRows = data.MappedRows<Order>().ToList();

        Assert.That(mappedRows.Count, Is.EqualTo(1));
        Assert.That(mappedRows[0].Row, Is.SameAs(row1));
        Assert.That(mappedRows[0].Result, Is.SameAs(order1));

        data.Add(new MappedRow<Order>(row2, order2));

        mappedRows = data.MappedRows<Order>().ToList();

        Assert.That(mappedRows.Count, Is.EqualTo(1));
        Assert.That(mappedRows[0].Row, Is.SameAs(row2));
        Assert.That(mappedRows[0].Result, Is.SameAs(order2));
    }

    [Test]
    public void Should_be_able_overwrite_a_list_of_mapped_rows()
    {
        MappedData data = new();
        var row1 = CreateDataRow();
        var row2 = CreateDataRow();
        var row3 = CreateDataRow();
        var row4 = CreateDataRow();
        Order order1 = new("ON-1", DateTime.Now);
        Order order2 = new("ON-2", DateTime.Now);
        Order order3 = new("ON-3", DateTime.Now);
        Order order4 = new("ON-4", DateTime.Now);

        data.Add(new List<MappedRow<Order>> { new(row1, order1), new(row2, order2) });

        var mappedRows = data.MappedRows<Order>().ToList();

        Assert.That(mappedRows.Count, Is.EqualTo(2));
        Assert.That(mappedRows[0].Row, Is.SameAs(row1));
        Assert.That(mappedRows[0].Result, Is.SameAs(order1));
        Assert.That(mappedRows[1].Row, Is.SameAs(row2));
        Assert.That(mappedRows[1].Result, Is.SameAs(order2));

        mappedRows = data.MappedRows<Order>(mr => mr.Result.OrderNumber.Contains("2")).ToList();

        Assert.That(mappedRows.Count, Is.EqualTo(1));
        Assert.That(mappedRows[0].Row, Is.SameAs(row2));
        Assert.That(mappedRows[0].Result, Is.SameAs(order2));

        data.Add(new List<MappedRow<Order>> { new(row3, order3), new(row4, order4) });

        mappedRows = data.MappedRows<Order>().ToList();

        Assert.That(mappedRows.Count, Is.EqualTo(2));
        Assert.That(mappedRows[0].Row, Is.SameAs(row3));
        Assert.That(mappedRows[0].Result, Is.SameAs(order3));
        Assert.That(mappedRows[1].Row, Is.SameAs(row4));
        Assert.That(mappedRows[1].Result, Is.SameAs(order4));
    }

    [Test]
    public void Should_be_able_to_get_empty_list_for_non_existent_items()
    {
        MappedData data = new();

        var mappedRows = data.MappedRows<Order>().ToList();

        Assert.That(mappedRows.Count, Is.Zero);
    }

    private DataRow CreateDataRow()
    {
        return new DataTable().NewRow();
    }
}