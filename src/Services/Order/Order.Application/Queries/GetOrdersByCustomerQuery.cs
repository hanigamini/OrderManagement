using MediatR;
using Order.Application.Abstractions;
using Order.Application.DTOs;

namespace Order.Application.Queries;

// CQRS - Query برای دریافت لیست سفارشات یک مشتری
public record GetOrdersByCustomerQuery(string CustomerId) : IRequest<List<OrderDto>>;

public class GetOrdersByCustomerQueryHandler(IOrderRepository repository)
    : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await repository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return orders
            .Select(o => new OrderDto(o.Id, o.Status.ToString(), o.TotalAmount))
            .ToList();
    }
}
