namespace Order.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public string ProductId { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem() { } // برای EF Core

    public OrderItem(string productId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public decimal Total => Quantity * UnitPrice;
}
