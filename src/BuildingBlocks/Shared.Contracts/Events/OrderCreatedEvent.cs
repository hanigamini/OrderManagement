namespace Shared.Contracts.Events;

// این پیام توسط Order.Service پس از ثبت سفارش به RabbitMQ ارسال می‌شود
// و توسط Payment.Service مصرف می‌شود.
public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public string CustomerId { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
