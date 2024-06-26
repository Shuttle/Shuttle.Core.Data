﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data.Tests.Fakes
{
	public class OrderAssembler : IAssembler<Order>
	{
		public IEnumerable<Order> Assemble(MappedData data)
		{
			var result = new List<Order>();

			foreach (var orderRow in data.MappedRows<Order>())
			{
				var order = orderRow.Result;

				foreach (var orderLineRow in data.MappedRows<OrderLine>())
				{
					if (orderLineRow.Row["OrderNumber"].Equals(order.OrderNumber))
					{
						order.AddLine(orderLineRow.Result);
					}
				}

				result.Add(order);
			}

			return result;
		}

		public async Task<IEnumerable<Order>> AssembleAsync(MappedData data, CancellationToken cancellationToken)
		{
			return await Task.FromResult(Assemble(data));
		}
	}
}