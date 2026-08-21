namespace Shared.Contracts.Events;

// این پیام توسط Payment.Service پس از پردازش موفق پرداخت منتشر می‌شود
// و توسط Order.Service مصرف می‌شود تا وضعیت سفارش را به Paid تغییر دهد.
public record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public bool IsSuccessful { get; init; }
    public string PaymentMethod { get; init; } = default!;
    public DateTime ProcessedAtUtc { get; init; }
}
