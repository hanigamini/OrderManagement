using MassTransit;
using Order.Application.Abstractions;
using Shared.Contracts.Events;

namespace Order.Application.EventHandlers;

// این Consumer پیام PaymentProcessedEvent را از Payment.Service دریافت می‌کند
// و در صورت موفقیت، وضعیت سفارش را به Paid تغییر می‌دهد.
public class PaymentProcessedEventHandler(IOrderRepository repository) : IConsumer<PaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;
        var order = await repository.GetByIdAsync(message.OrderId, context.CancellationToken);
        if (order is null) return;

        if (message.IsSuccessful)
        {
            order.MarkAsPaid();
            await repository.UpdateAsync(order, context.CancellationToken);
        }
    }
}
