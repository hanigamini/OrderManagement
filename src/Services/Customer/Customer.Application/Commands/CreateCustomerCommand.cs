using MediatR;
using Customer.Application.DTOs;
using Customer.Domain.Entities;
using Customer.Application.Abstractions;

namespace Customer.Application.Commands;

// CQRS - Command برای ثبت مشتری جدید
public record CreateCustomerCommand(string FullName, string Email, string PhoneNumber)
    : IRequest<CustomerDto>;

public class CreateCustomerCommandHandler(ICustomerRepository repository)
    : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Domain.Entities.Customer(request.FullName, request.Email, request.PhoneNumber);
        await repository.AddAsync(customer, cancellationToken);
        return new CustomerDto(customer.Id, customer.FullName, customer.Email, customer.PhoneNumber);
    }
}
