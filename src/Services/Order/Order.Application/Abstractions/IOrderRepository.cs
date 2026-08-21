namespace Order.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Domain.Entities.Order order, CancellationToken ct);
    Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Domain.Entities.Order>> GetByCustomerIdAsync(string customerId, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Order order, CancellationToken ct);
}
