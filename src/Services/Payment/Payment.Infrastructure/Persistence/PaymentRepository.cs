using Payment.Application.Abstractions;

namespace Payment.Infrastructure.Persistence;

public class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
{
    public async Task AddAsync(Domain.Entities.Payment payment, CancellationToken ct)
    {
        await dbContext.Payments.AddAsync(payment, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}
