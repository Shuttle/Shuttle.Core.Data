using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shuttle.Core.Data.Tests.Fakes;

public class OrderAssembler : IAssembler<Order>
{
    public async Task<IEnumerable<Order>> AssembleAsync(MappedData data, CancellationToken cancellationToken = default)
    {
        List<Order> result = new();

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

        return await Task.FromResult(result);
    }
}