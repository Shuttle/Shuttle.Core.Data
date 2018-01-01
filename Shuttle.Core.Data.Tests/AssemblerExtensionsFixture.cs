using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Shuttle.Core.Data.Tests.Fakes;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class AssemblerExtensionsFixture
	{
		[Test]
		public void Should_be_able_to_assemble_item()
		{
			var now = DateTime.Now;
			var mappedData = new MappedData();

			var orderTable = new DataTable();

			orderTable.Columns.Add("OrderNumber", typeof(string));
			orderTable.Columns.Add("OrderDate", typeof(DateTime));

			const string orderNumber = "ON-10";

			mappedData.Add(new MappedRow<Order>(orderTable.Rows.Add(orderNumber, now), new Order(orderNumber, now)));

			var orderLineTable = new DataTable();

			orderLineTable.Columns.Add("OrderNumber", typeof(string));
			orderLineTable.Columns.Add("ProductId", typeof(string));
			orderLineTable.Columns.Add("Quantity", typeof(int));
			orderLineTable.Columns.Add("UnitCost", typeof(double));

			mappedData.Add(new List<MappedRow<OrderLine>>
				{
					new MappedRow<OrderLine>(orderLineTable.Rows.Add(orderNumber, "SKU-1", 5, 10), new OrderLine("SKU-1", 5, 10)),
					new MappedRow<OrderLine>(orderLineTable.Rows.Add(orderNumber, "SKU-2", 1, 65), new OrderLine("SKU-2", 1, 65)),
					new MappedRow<OrderLine>(orderLineTable.Rows.Add(orderNumber, "SKU-3", 10, 10.5), new OrderLine("SKU-3", 10, 10.5))
				});

			var order = new OrderAssembler().AssembleItem(mappedData);

			Assert.AreEqual(orderNumber, order.OrderNumber);
			Assert.AreEqual(now, order.OrderDate);
			Assert.AreEqual(3, order.Lines.Count());
			Assert.AreEqual(50, order.Lines.ElementAt(0).TotalCost());
			Assert.AreEqual(65, order.Lines.ElementAt(1).TotalCost());
			Assert.AreEqual(105, order.Lines.ElementAt(2).TotalCost());
			Assert.AreEqual(220, order.Total());
		}
	}
}