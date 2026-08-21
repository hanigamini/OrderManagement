namespace Payment.Application.Abstractions;

public interface IPaymentRepository
{
    Task AddAsync(Domain.Entities.Payment payment, CancellationToken ct);
}
