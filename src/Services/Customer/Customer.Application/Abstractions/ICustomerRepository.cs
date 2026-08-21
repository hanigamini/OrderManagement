namespace Customer.Application.Abstractions;

public interface ICustomerRepository
{
    Task AddAsync(Domain.Entities.Customer customer, CancellationToken ct);
    Task<List<Domain.Entities.Customer>> GetAllAsync(CancellationToken ct);
}
