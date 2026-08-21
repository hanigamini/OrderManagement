using MediatR;
using Customer.Application.DTOs;
using Customer.Application.Abstractions;

namespace Customer.Application.Queries;

// CQRS - Query برای دریافت لیست مشتریان
public record GetCustomersQuery : IRequest<List<CustomerDto>>;

public class GetCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await repository.GetAllAsync(cancellationToken);
        return customers
            .Select(c => new CustomerDto(c.Id, c.FullName, c.Email, c.PhoneNumber))
            .ToList();
    }
}
