using MediatR;
using Payment.Application.Abstractions;

namespace Payment.Application.Commands;

// CQRS - Command برای پردازش پرداخت یک سفارش (فراخوانی مستقیم از طریق API)
public record ProcessPaymentCommand(Guid OrderId, string PaymentMethod, decimal Amount)
    : IRequest<string>;

public class ProcessPaymentCommandHandler(IPaymentRepository repository)
    : IRequestHandler<ProcessPaymentCommand, string>
{
    public async Task<string> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Domain.Entities.Payment(request.OrderId, request.PaymentMethod, request.Amount);

        // منطق نمونه پرداخت - در دنیای واقعی اینجا به درگاه پرداخت متصل می‌شوید
        payment.MarkAsSuccessful();

        await repository.AddAsync(payment, cancellationToken);
        return "Success";
    }
}
