using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Shuttle.Core.Data.Tests.Fakes;

namespace Shuttle.Core.Data.Tests
{
	[TestFixture]
	public class MappedDataTests
	{
		[Test]
		public void Should_be_able_to_add_and_overwrite_a_single_mapped_row()
		{
			var data = new MappedData();
			var row1 = CreateDataRow();
			var row2 = CreateDataRow();
			var order1 = new Order("ON-1", DateTime.Now);
			var order2 = new Order("ON-2", DateTime.Now);

			data.Add(new MappedRow<Order>(row1, order1));

			var mappedRows = data.MappedRows<Order>().ToList();

			Assert.AreEqual(1, mappedRows.Count);
			Assert.AreSame(row1, mappedRows[0].Row);
			Assert.AreSame(order1, mappedRows[0].Result);

			data.Add(new MappedRow<Order>(row2, order2));

			mappedRows = data.MappedRows<Order>().ToList();

			Assert.AreEqual(1, mappedRows.Count);
			Assert.AreSame(row2, mappedRows[0].Row);
			Assert.AreSame(order2, mappedRows[0].Result);
		}

		[Test]
		public void Should_be_able_overwrite_a_list_of_mapped_rows()
		{
			var data = new MappedData();
			var row1 = CreateDataRow();
			var row2 = CreateDataRow();
			var row3 = CreateDataRow();
			var row4 = CreateDataRow();
			var order1 = new Order("ON-1", DateTime.Now);
			var order2 = new Order("ON-2", DateTime.Now);
			var order3 = new Order("ON-3", DateTime.Now);
			var order4 = new Order("ON-4", DateTime.Now);

			data.Add(new List<MappedRow<Order>>
				{
					new MappedRow<Order>(row1, order1),
					new MappedRow<Order>(row2, order2)
				});

			var mappedRows = data.MappedRows<Order>().ToList();

			Assert.AreEqual(2, mappedRows.Count);
			Assert.AreSame(row1, mappedRows[0].Row);
			Assert.AreSame(order1, mappedRows[0].Result);
			Assert.AreSame(row2, mappedRows[1].Row);
			Assert.AreSame(order2, mappedRows[1].Result);

			mappedRows = data.MappedRows<Order>(mr => mr.Result.OrderNumber.Contains("2")).ToList();

			Assert.AreEqual(1, mappedRows.Count);
			Assert.AreSame(row2, mappedRows[0].Row);
			Assert.AreSame(order2, mappedRows[0].Result);

			data.Add(new List<MappedRow<Order>>
				{
					new MappedRow<Order>(row3, order3),
					new MappedRow<Order>(row4, order4)
				});

			mappedRows = data.MappedRows<Order>().ToList();

			Assert.AreEqual(2, mappedRows.Count);
			Assert.AreSame(row3, mappedRows[0].Row);
			Assert.AreSame(order3, mappedRows[0].Result);
			Assert.AreSame(row4, mappedRows[1].Row);
			Assert.AreSame(order4, mappedRows[1].Result);
		}

		[Test]
		public void Should_be_able_to_get_empty_list_for_non_existent_items()
		{
			var data = new MappedData();

			var mappedRows = data.MappedRows<Order>().ToList();

			Assert.AreEqual(0, mappedRows.Count);
		}

		private DataRow CreateDataRow()
		{
			return new DataTable().NewRow();
		}
	}
}