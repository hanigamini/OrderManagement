using MassTransit;
using Payment.Application.Abstractions;
using Shared.Contracts.Events;

namespace Payment.Application.Consumers;

// این Consumer پیام OrderCreatedEvent را از Order.Service دریافت می‌کند،
// پرداخت را پردازش کرده و در صورت موفقیت PaymentProcessedEvent را منتشر می‌کند.
public class OrderCreatedEventConsumer(IPaymentRepository repository) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var payment = new Domain.Entities.Payment(message.OrderId, "CreditCard", message.TotalAmount);

        // منطق نمونه پرداخت - در دنیای واقعی اینجا به درگاه پرداخت متصل می‌شوید
        payment.MarkAsSuccessful();
        await repository.AddAsync(payment, context.CancellationToken);

        await context.Publish(new PaymentProcessedEvent
        {
            OrderId = message.OrderId,
            IsSuccessful = payment.Status == Domain.Entities.PaymentStatus.Success,
            PaymentMethod = payment.PaymentMethod,
            ProcessedAtUtc = DateTime.UtcNow
        });
    }
}
