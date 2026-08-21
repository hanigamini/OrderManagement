using MassTransit;
using MediatR;
using Order.Application.Abstractions;
using Order.Application.DTOs;
using Order.Domain.Entities;
using Shared.Contracts.Events;

namespace Order.Application.Commands;

// CQRS - Command برای ثبت سفارش جدید
public record CreateOrderCommand(string CustomerId, List<CreateOrderItemDto> Items)
    : IRequest<OrderDto>;

public class CreateOrderCommandHandler(
    IOrderRepository repository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new OrderItem(i.ProductId, i.Quantity, i.UnitPrice));
        var order = new Domain.Entities.Order(request.CustomerId, items);

        await repository.AddAsync(order, cancellationToken);

        // پس از ثبت سفارش، پیام به RabbitMQ ارسال می‌شود تا Payment.Service آن را پردازش کند
        await publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc
        }, cancellationToken);

        return new OrderDto(order.Id, order.Status.ToString(), order.TotalAmount);
    }
}
