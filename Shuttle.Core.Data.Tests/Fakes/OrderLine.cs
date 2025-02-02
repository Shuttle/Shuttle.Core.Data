namespace Shuttle.Core.Data.Tests.Fakes;

public class OrderLine
{
    public OrderLine(string productId, int quantity, double unitCost)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
    }

    public string ProductId { get; private set; }
    public int Quantity { get; }
    public double UnitCost { get; }

    public double TotalCost()
    {
        return UnitCost * Quantity;
    }
}