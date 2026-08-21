using Microsoft.EntityFrameworkCore;
using Customer.Application.Abstractions;

namespace Customer.Infrastructure.Persistence;

public class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
    public async Task AddAsync(Domain.Entities.Customer customer, CancellationToken ct)
    {
        await dbContext.Customers.AddAsync(customer, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<List<Domain.Entities.Customer>> GetAllAsync(CancellationToken ct)
        => await dbContext.Customers.AsNoTracking().ToListAsync(ct);
}
