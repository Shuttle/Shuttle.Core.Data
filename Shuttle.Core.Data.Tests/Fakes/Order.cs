using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shuttle.Core.Data.Tests.Fakes
{
	public class Order
	{
		public string OrderNumber { get; private set; }
		public DateTime OrderDate { get; private set; }

		private readonly List<OrderLine> _lines = new List<OrderLine>();

		public Order(string orderNumber, DateTime orderDate)
		{
			OrderNumber = orderNumber;
			OrderDate = orderDate;
		}

		public void AddLine(OrderLine line)
		{
			_lines.Add(line);
		}

		public double Total()
		{
			return _lines.Sum(line => line.TotalCost());
		}

		public IEnumerable<OrderLine> Lines
		{
			get
			{
				return new ReadOnlyCollection<OrderLine>(_lines);
			}
		}
	}
}