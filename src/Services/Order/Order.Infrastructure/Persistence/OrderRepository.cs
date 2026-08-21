using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;

namespace Order.Infrastructure.Persistence;

public class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order.Domain.Entities.Order order, CancellationToken ct)
    {
        await dbContext.Orders.AddAsync(order, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Order.Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<List<Order.Domain.Entities.Order>> GetByCustomerIdAsync(string customerId, CancellationToken ct)
        => await dbContext.Orders.Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task UpdateAsync(Order.Domain.Entities.Order order, CancellationToken ct)
    {
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(ct);
    }
}
