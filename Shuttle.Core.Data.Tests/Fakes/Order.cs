using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shuttle.Core.Data.Tests.Fakes;

public class Order
{
    private readonly List<OrderLine> _lines = new();

    public Order(string orderNumber, DateTime orderDate)
    {
        OrderNumber = orderNumber;
        OrderDate = orderDate;
    }

    public IEnumerable<OrderLine> Lines => new ReadOnlyCollection<OrderLine>(_lines);

    public DateTime OrderDate { get; private set; }
    public string OrderNumber { get; private set; }

    public void AddLine(OrderLine line)
    {
        _lines.Add(line);
    }

    public double Total()
    {
        return _lines.Sum(line => line.TotalCost());
    }
}