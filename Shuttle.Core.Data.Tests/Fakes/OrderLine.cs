namespace Shuttle.Core.Data.Tests.Fakes
{
	public class OrderLine
	{
		public string ProductId { get; private set; }
		public int Quantity { get; private set; }
		public double UnitCost { get; private set; }

		public OrderLine(string productId, int quantity, double unitCost)
		{
			ProductId = productId;
			Quantity = quantity;
			UnitCost = unitCost;
		}

		public double TotalCost()
		{
			return UnitCost*Quantity;
		}
	}
}