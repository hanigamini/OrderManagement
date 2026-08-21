using Order.Domain.Enums;

namespace Order.Domain.Entities;

// Domain Model اصلی این سرویس - قوانین تغییر وضعیت سفارش اینجا نگهداری می‌شود (DDD)
public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public string CustomerId { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.Total);

    private Order() { } // برای EF Core

    public Order(string customerId, IEnumerable<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("شناسه مشتری الزامی است.", nameof(customerId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        _items.AddRange(items);

        if (_items.Count == 0)
            throw new InvalidOperationException("سفارش باید حداقل یک آیتم داشته باشد.");
    }

    // قانون دامنه: مسیر مجاز تغییر وضعیت فقط Pending -> Paid -> Shipped است
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"امکان تغییر وضعیت از {Status} به Paid وجود ندارد.");
        Status = OrderStatus.Paid;
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"امکان تغییر وضعیت از {Status} به Shipped وجود ندارد.");
        Status = OrderStatus.Shipped;
    }
}
