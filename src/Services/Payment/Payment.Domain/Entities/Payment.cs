namespace Payment.Domain.Entities;

public enum PaymentStatus { Pending, Success, Failed }

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string PaymentMethod { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Payment() { } // برای EF Core

    public Payment(Guid orderId, string paymentMethod, decimal amount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsSuccessful() => Status = PaymentStatus.Success;
    public void MarkAsFailed() => Status = PaymentStatus.Failed;
}
